using Kaleido.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Kaleido;

public sealed class RecordDispatcher : IRecordDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRecordRegistry _registry;
    private readonly IRecordDescriptorFactory _descriptors;

    public RecordDispatcher(IServiceScopeFactory scopeFactory, IRecordRegistry registry, IRecordDescriptorFactory descriptors)
    {
        _scopeFactory = scopeFactory;
        _registry = registry;
        _descriptors = descriptors;
    }

    public async Task<KaleidoQueryResponse> QueryAsync(string recordKey, KaleidoQueryRequest request, CancellationToken cancellationToken = default)
    {
        var registration = _registry.Find(recordKey)
            ?? throw new KeyNotFoundException($"Record '{recordKey}' is not registered.");

        var engineType = typeof(IRecordQueryEngine<>).MakeGenericType(registration.RecordType);
        using var scope = _scopeFactory.CreateScope();
        var engine = (IRecordQueryEngine)scope.ServiceProvider.GetRequiredService(engineType);

        var result = await engine.ExecuteAsync(request, cancellationToken);

        return new KaleidoQueryResponse(
            _descriptors.Create(result.RuntimeMetadata),
            result.TotalCount,
            result.ItemsAsObjects);
    }

    public async Task<KaleidoQueryResponse<TRecord>> QueryAsync<TRecord>(string recordKey, KaleidoQueryRequest request, CancellationToken cancellationToken = default) where TRecord : class
    {
        var registration = _registry.Find(recordKey)
            ?? throw new KeyNotFoundException($"Record '{recordKey}' is not registered.");

        var engineType = typeof(IRecordQueryEngine<TRecord>).MakeGenericType(registration.RecordType);
        using var scope = _scopeFactory.CreateScope();
        var engine = (IRecordQueryEngine<TRecord>)scope.ServiceProvider.GetRequiredService(engineType);

        var result = await engine.ExecuteTypedAsync(request, cancellationToken);

        return new KaleidoQueryResponse<TRecord>(
            _descriptors.Create(result.RuntimeMetadata),
            result.TotalCount,
            result.Items);
    }
}
