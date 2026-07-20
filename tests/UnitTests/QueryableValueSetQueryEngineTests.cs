using Kaleido;
using Kaleido.Queryable;
using Xunit;
namespace Queryable.Tests;

public sealed class QueryableValueSetQueryEngineTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Compose_Pipeline()
    {
        var engine = new QueryableValueSetQueryEngine<ClientRecord>(new ValueSetMetadataCatalog(), new ValueSetQueryValidator(), new ValueSetQueryCompiler(), new Source(), new IQueryableValueSetNamedQuery<ClientRecord>[] { new Active() }, new QueryableCompiledQueryApplier<ClientRecord>(), new QueryableValueSetExecutor<ClientRecord>());
        var request = new QueryRequest("active", new QueryBody(new QuerySearch("Blue", MatchMode.StartsWith), new QueryFilter(nameof(ClientRecord.GroupName), FilterOperator.Eq, new List<object?> { "Commercial" }), new[] { new QuerySort(nameof(ClientRecord.ClientName), SortDirection.Desc) }, new QueryPage(1)));
        var result = await engine.ExecuteAsync(request);
        Assert.Single(result.Items);
        Assert.Equal("Blue Cross", result.Items[0].ClientName);
    }

    private sealed class Source : IQueryableValueSetSource<ClientRecord>
    {
        public IQueryable<ClientRecord> CreateQuery(ValueSetExecutionContext context) => ClientRecords.All.AsQueryable();
    }

    private sealed class Active : IQueryableValueSetNamedQuery<ClientRecord>
    {
        public string Name => "active";
        public IQueryable<ClientRecord> Apply(IQueryable<ClientRecord> query, IReadOnlyDictionary<string, object?>? parameters) => query.Where(x => x.IsActive);
    }
}
