using System.Text.Json;
using System.Text.Json.Nodes;
using Kaleido.Extensions;

namespace Kaleido.CsvFunctionalTests;

public static class JsonOptionsProvider
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}

public static class FunctionalTestCaseProvider
{
    public static IEnumerable<object[]> AllCases()
    {
        var path = PathResolver.ResolveRequestFile("functional-cases.json");
        var json = File.ReadAllText(path);
        var cases = JsonSerializer.Deserialize<List<FunctionalTestCase>>(json, JsonOptionsProvider.Default)
            ?? throw new InvalidOperationException("Could not deserialize functional test cases.");

        foreach (var testCase in cases)
        {
            yield return new object[] { testCase };
        }
    }
}

public static class QueryRequestFactory
{
    public static KaleidoQueryRequest Create(JsonObject json)
    {
        var queryName = json["queryName"]?.GetValue<string?>();
        var query = json["query"] is JsonObject q ? CreateBody(q) : null;
        var parameters = json["parameters"] is JsonObject p
            ? p.ToDictionary(x => x.Key, x => ConvertJsonValue(x.Value))
            : null;

        return new KaleidoQueryRequest(queryName, query, parameters);
    }

    private static QueryBody CreateBody(JsonObject json)
    {
        var search = json["search"] is JsonObject s ? CreateSearch(s) : null;
        var filter = json["filter"] is JsonObject f ? CreateFilter(f) : null;
        var sort = json["sort"] is JsonArray sortArray ? sortArray.OfType<JsonObject>().Select(CreateSort).ToArray() : null;
        var page = json["page"] is JsonObject p ? new QueryPage(p["size"]?.GetValue<int?>(), p["offset"]?.GetValue<int?>()) : null;

        return new QueryBody(search, filter, sort, page);
    }

    private static ISearchExpression CreateSearch(JsonObject json)
    {
        if (json["expressions"] is JsonArray expressions)
        {
            return new QuerySearchGroup(ParseEnum<LogicalOperator>(json["operator"]?.GetValue<string>() ?? "or"), expressions.OfType<JsonObject>().Select(CreateSearch).ToList());
        }

        return new QuerySearch(
            json["searchText"]?.GetValue<string>() ?? string.Empty,
            ParseEnum<MatchMode>(json["matchMode"]?.GetValue<string>() ?? "contains"),
            json["field"]?.GetValue<string?>());
    }

    private static IFilterExpression CreateFilter(JsonObject json)
    {
        if (json["expressions"] is JsonArray expressions)
        {
            return new QueryFilterGroup(ParseEnum<LogicalOperator>(json["operator"]?.GetValue<string>() ?? "and"), expressions.OfType<JsonObject>().Select(CreateFilter).ToList());
        }

        var values = json["values"] is JsonArray v ? v.Select(ConvertJsonValue).ToArray() : Array.Empty<object?>();
        return new QueryFilter(
            json["field"]!.GetValue<string>(),
            ParseEnum<FilterOperator>(json["operator"]!.GetValue<string>()),
            values.ToList());
    }

    private static QuerySort CreateSort(JsonObject json)
    {
        return new QuerySort(
            json["field"]!.GetValue<string>(),
            ParseEnum<SortDirection>(json["direction"]?.GetValue<string>() ?? "asc"),
            json["sequence"]?.GetValue<int?>());
    }

    private static object? ConvertJsonValue(JsonNode? node)
    {
        if (node is null) return null;
        var value = node.AsValue();
        if (value.TryGetValue<bool>(out var boolValue)) return boolValue;
        if (value.TryGetValue<int>(out var intValue)) return intValue;
        if (value.TryGetValue<long>(out var longValue)) return longValue;
        if (value.TryGetValue<decimal>(out var decimalValue)) return decimalValue;
        if (value.TryGetValue<double>(out var doubleValue)) return doubleValue;
        if (value.TryGetValue<string>(out var stringValue)) return stringValue;
        return node.ToJsonString();
    }

    private static TEnum ParseEnum<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (EnumExtensions.TryParseFromDescription<TEnum>(value, out var result))
        {
            return result;
        }

        return Enum.Parse<TEnum>(value, ignoreCase: true);
    }
}
