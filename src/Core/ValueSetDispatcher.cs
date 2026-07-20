using Kaleido.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Kaleido;

public sealed class ValueSetDispatcher : IValueSetDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IValueSetRegistry _registry;
    private readonly IValueSetDescriptorFactory _descriptors;

    public ValueSetDispatcher(IServiceScopeFactory scopeFactory, IValueSetRegistry registry, IValueSetDescriptorFactory descriptors)
    {
        _scopeFactory = scopeFactory;
        _registry = registry;
        _descriptors = descriptors;
    }

    public async Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default)
    {
        var registration = _registry.Find(valueSetKey)
            ?? throw new KeyNotFoundException($"Value set '{valueSetKey}' is not registered.");

        var engineType = typeof(IValueSetQueryEngine<>).MakeGenericType(registration.RecordType);
        var scope = _scopeFactory.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService(engineType);
        var method = engineType.GetMethod(nameof(IValueSetQueryEngine<object>.ExecuteAsync))
            ?? throw new InvalidOperationException($"Could not find ExecuteAsync on '{engineType}'.");

        var task = (Task)method.Invoke(engine, new object?[] { request, cancellationToken })!;
        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException("Could not get query result from engine task.");

        var result = (IValueSetQueryResult)resultProperty.GetValue(task)!;

        return new ValueSetQueryResponse(
            _descriptors.Create(result.RuntimeMetadata),
            result.TotalCount,
            result.NextCursor,
            result.ItemsAsObjects);
    }
}
