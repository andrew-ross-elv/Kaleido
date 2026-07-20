using Kaleido.Extensions;
using Kaleido.Queryable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kaleido.CsvFunctionalTests;

public sealed class FunctionalRecordSource : IQueryableValueSetSource<FunctionalRecord>
{
    private readonly FunctionalRecordStore _store;

    public FunctionalRecordSource(FunctionalRecordStore store)
    {
        _store = store;
    }

    public IQueryable<FunctionalRecord> CreateQuery(ValueSetExecutionContext context)
    {
        return _store.Records.AsQueryable();
    }
}

public sealed class FunctionalRecordStore
{
    public FunctionalRecordStore()
    {
        Records = FunctionalCsvLoader.Load(PathResolver.ResolveDataFile("functional-records.csv"));
    }

    public IReadOnlyList<FunctionalRecord> Records { get; }
}

public static class FunctionalCsvLoader
{
    public static IReadOnlyList<FunctionalRecord> Load(string path)
    {
        var lines = File.ReadAllLines(path);
        if (lines.Length <= 1) return Array.Empty<FunctionalRecord>();

        var header = SplitCsvLine(lines[0]);
        var headerIndex = header.Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index, StringComparer.OrdinalIgnoreCase);

        var records = new List<FunctionalRecord>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var columns = SplitCsvLine(line);
            string Get(string name) => columns[headerIndex[name]];
            string? GetNullable(string name) => string.IsNullOrWhiteSpace(Get(name)) ? null : Get(name);

            records.Add(new FunctionalRecord
            {
                Id = int.Parse(Get("Id")),
                ExternalId = Guid.Parse(Get("ExternalId")),
                Code = Get("Code"),
                Name = Get("Name"),
                Category = Get("Category"),
                IsActive = bool.Parse(Get("IsActive")),
                Quantity = int.Parse(Get("Quantity")),
                Amount = decimal.Parse(Get("Amount")),
                Rate = double.Parse(Get("Rate")),
                Score = float.Parse(Get("Score")),
                EffectiveDate = DateOnly.Parse(Get("EffectiveDate")),
                CreatedAt = DateTime.Parse(Get("CreatedAt")),
                ExpirationDate = GetNullable("ExpirationDate") is { } expiration ? DateOnly.Parse(expiration) : null,
                Status = EnumExtensions.TryParseFromDescription<FunctionalStatus>(Get("Status"), out var status) ? status : Enum.Parse<FunctionalStatus>(Get("Status"), true),
                Priority = int.Parse(Get("Priority")),
                Region = Get("Region"),
                GroupName = Get("GroupName"),
                Version = long.Parse(Get("Version")),
                Notes = Get("Notes"),
                NullableScore = GetNullable("NullableScore") is { } nullableScore ? float.Parse(nullableScore) : null
            });
        }

        return records;
    }

    private static string[] SplitCsvLine(string line)
    {
        // This test data intentionally avoids embedded commas/quotes.
        return line.Split(',');
    }
}

public static class PathResolver
{
    public static string ResolveDataFile(string fileName)
    {
        var output = Path.Combine(AppContext.BaseDirectory, "data", fileName);
        if (File.Exists(output)) return output;

        var source = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data", fileName));
        if (File.Exists(source)) return source;

        throw new FileNotFoundException($"Could not find data file '{fileName}'. Checked '{output}' and '{source}'.");
    }

    public static string ResolveRequestFile(string fileName)
    {
        var output = Path.Combine(AppContext.BaseDirectory, "requests", fileName);
        if (File.Exists(output)) return output;

        var source = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "requests", fileName));
        if (File.Exists(source)) return source;

        throw new FileNotFoundException($"Could not find request file '{fileName}'. Checked '{output}' and '{source}'.");
    }
}
