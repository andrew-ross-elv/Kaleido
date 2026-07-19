using Microsoft.Extensions.DependencyInjection;
using Kaleido.Abstractions;

namespace Kaleido.Core;

public sealed class ValueSetDispatcher : IValueSetDispatcher
{
    private readonly IServiceProvider _services;
    private readonly IValueSetRegistry _registry;
    private readonly IValueSetDescriptorFactory _descriptors;

    public ValueSetDispatcher(IServiceProvider services, IValueSetRegistry registry, IValueSetDescriptorFactory descriptors)
    {
        _services = services;
        _registry = registry;
        _descriptors = descriptors;
    }

    public async Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default)
    {
        var registration = _registry.Find(valueSetKey)
            ?? throw new KeyNotFoundException($"Value set '{valueSetKey}' is not registered.");

        var engineType = typeof(IValueSetQueryEngine<>).MakeGenericType(registration.RecordType);
        var engine = _services.GetRequiredService(engineType);
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
