using System;
using System.Collections.Generic;
using System.Text;

namespace Kaleido.Metadata;

/// <summary>
/// Central metadata cache for all registered value set record types.
///
/// This service is responsible for converting record definitions
/// (ValueSetAttribute, FilterableAttribute, SearchableAttribute, etc.)
/// into RuntimeValueSetMetadata instances.
///
/// Metadata is generated once and reused for the lifetime of the
/// application to avoid repeated reflection operations.
/// </summary>
/// <remarks>
/// Architecture Role:
///     Metadata Provider
///
/// Used By:
///     - ValueSetQueryEngine<T>
///     - ValueSetDescriptorFactory
///     - Registration/Discovery components
///
/// Implementations:
///     - ValueSetMetadataCatalog
///
/// Replaceability:
///     Consumers should depend on this abstraction only.
///     Alternate implementations may load metadata from
///     external registries or precompiled sources.
/// </remarks>
public interface IValueSetMetadataCatalog
{
    RuntimeValueSetMetadata GetMetadata<TRecord>() where TRecord : class;
    RuntimeValueSetMetadata GetMetadata(Type recordType);
}
