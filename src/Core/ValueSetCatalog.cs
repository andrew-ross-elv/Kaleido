using Kaleido.Abstractions;

namespace Kaleido.Core;

public sealed class ValueSetCatalog : IValueSetCatalog
{
    private readonly IValueSetRegistry _registry;
    private readonly IValueSetDispatcher _dispatcher;
    private readonly IValueSetDescriptorFactory _descriptors;

    public ValueSetCatalog(IValueSetRegistry registry, IValueSetDispatcher dispatcher, IValueSetDescriptorFactory descriptors)
    {
        _registry = registry;
        _dispatcher = dispatcher;
        _descriptors = descriptors;
    }

    public IReadOnlyCollection<ValueSetDescriptor> GetAll()
    {
        return _registry.Registrations.Select(x => _descriptors.Create(x.RuntimeMetadata)).ToArray();
    }

    public ValueSetDescriptor? Get(string valueSetKey)
    {
        var registration = _registry.Find(valueSetKey);
        return registration is null ? null : _descriptors.Create(registration.RuntimeMetadata);
    }

    public Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(valueSetKey, request, cancellationToken);
    }
}
