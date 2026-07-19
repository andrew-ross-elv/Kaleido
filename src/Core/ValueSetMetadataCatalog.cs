using System.Collections.Concurrent;
using Kaleido.Abstractions;

namespace Kaleido.Core;

public sealed class ValueSetMetadataCatalog : IValueSetMetadataCatalog
{
    private readonly ConcurrentDictionary<Type, RuntimeValueSetMetadata> _cache = new();
    public RuntimeValueSetMetadata GetMetadata<TRecord>() where TRecord : class => GetMetadata(typeof(TRecord));
    public RuntimeValueSetMetadata GetMetadata(Type recordType) => _cache.GetOrAdd(recordType, ValueSetMetadataBuilder.Build);
}
