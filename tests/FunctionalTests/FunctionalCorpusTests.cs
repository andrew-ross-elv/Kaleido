using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Kaleido.CsvFunctionalTests;

[Collection(CsvFunctionalCollection.Name)]
public sealed class FunctionalCorpusTests
{
    private readonly CsvFunctionalFixture _fixture;

    public FunctionalCorpusTests(CsvFunctionalFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(GetCases))]
    public async Task Functional_Request_Should_Match_Expected_Result(FunctionalTestCase testCase)
    {
        var request = QueryRequestFactory.Create(testCase.Request);

        if (testCase.ExpectedFailure)
        {
            await Assert.ThrowsAnyAsync<Exception>(() => _fixture.QueryAsync(testCase.RecordKey, request));
            return;
        }

        var actual = await _fixture.QueryAsync(testCase.RecordKey, request);
        Assert.NotNull(testCase.Expected);
        Assert.Equal(testCase.Expected.ExpectedTotalCount, actual.TotalCount);
        Assert.Equal(testCase.Expected.ExpectedReturnedCount, actual.Items.Count);

        var actualKeys = actual.Items.Select(ReadId).ToArray();
        Assert.Equal(testCase.Expected.ExpectedKeys, actualKeys);
    }

    public static IEnumerable<object[]> GetCases() => FunctionalTestCaseProvider.AllCases();

    private static string ReadId(object item)
    {
        var property = item.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property is not null)
        {
            return Convert.ToString(property.GetValue(item)) ?? string.Empty;
        }

        var node = JsonSerializer.SerializeToNode(item, JsonOptionsProvider.Default);
        if (node is JsonObject obj && obj["Id"] is not null)
        {
            return obj["Id"]!.ToString();
        }

        throw new InvalidOperationException($"Could not read Id from response item type '{item.GetType().FullName}'.");
    }
}
