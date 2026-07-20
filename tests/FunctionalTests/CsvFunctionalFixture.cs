using Kaleido;
using Microsoft.Extensions.DependencyInjection;

namespace Kaleido.CsvFunctionalTests;

public sealed class CsvFunctionalFixture : IDisposable
{
    private readonly ServiceProvider _provider;

    public CsvFunctionalFixture()
    {
        var services = new ServiceCollection();

        services.AddSingleton<FunctionalRecordStore>();

        // Adjust if your actual registration extensions differ.
        services.AddKaleido();
        services.AddQueryableValueSetsFromAssembly(typeof(FunctionalRecord).Assembly);

        _provider = services.BuildServiceProvider(validateScopes: true);
    }

    public async Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request)
    {
        using var scope = _provider.CreateScope();
        var catalog = scope.ServiceProvider.GetRequiredService<IValueSetCatalog>();
        return await catalog.QueryAsync(valueSetKey, request);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}

[CollectionDefinition(Name)]
public sealed class CsvFunctionalCollection : ICollectionFixture<CsvFunctionalFixture>
{
    public const string Name = "CSV Functional Tests";
}
