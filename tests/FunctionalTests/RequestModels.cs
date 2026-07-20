using System.Text.Json.Nodes;

namespace Kaleido.CsvFunctionalTests;

public sealed record FunctionalTestCase(
    string Id,
    string Name,
    string ValueSetKey,
    string Category,
    bool ExpectedFailure,
    string? Notes,
    JsonObject Request,
    FunctionalExpectedResult? Expected);

public sealed record FunctionalExpectedResult(
    int ExpectedTotalCount,
    int ExpectedReturnedCount,
    IReadOnlyList<string> ExpectedKeys);
