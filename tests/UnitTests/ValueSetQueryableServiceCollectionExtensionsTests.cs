using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Kaleido.Queryable;
using Kaleido.Attributes;
using Kaleido;
namespace Queryable.Tests;

public sealed class ValueSetQueryableServiceCollectionExtensionsTests
{
    [Fact]
    public void AddQueryableValueSetsFromAssembly_Should_Register_Source_NamedQuery_And_Catalog()
    {
        var services = new ServiceCollection();
        services.AddQueryableValueSetsFromAssembly(typeof(ScanSource).Assembly);
        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IValueSetCatalog>());
        Assert.NotNull(provider.GetService<IQueryableValueSetSource<ScanRecord>>());
        Assert.Single(provider.GetServices<IQueryableValueSetNamedQuery<ScanRecord>>());
    }

    [ValueSet("Scan", "1", "Demo")]
    [AllowedQuery("all", "All")]
    private sealed class ScanRecord
    {
        [Filterable(FilterOperator.Eq)]
        public string Name { get; init; } = string.Empty;
    }

    private sealed class ScanSource : IQueryableValueSetSource<ScanRecord>
    {
        public IQueryable<ScanRecord> CreateQuery(ValueSetExecutionContext context) => new[] { new ScanRecord { Name = "A" } }.AsQueryable();
    }

    private sealed class ScanQuery : IQueryableValueSetNamedQuery<ScanRecord>
    {
        public string Name => "all";
        public IQueryable<ScanRecord> Apply(IQueryable<ScanRecord> query, IReadOnlyDictionary<string, object?>? parameters) => query;
    }
}
