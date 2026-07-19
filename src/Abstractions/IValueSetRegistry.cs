namespace Kaleido.Abstractions;

/// <summary>
/// Maintains the list of registered value sets available
/// to the application.
///
/// While IValueSetMetadataCatalog is responsible for generating
/// metadata for a specific record type, the registry is responsible
/// for discovering which value sets exist within the application.
///
/// Think of this component as the directory of available value sets.
/// </summary>
///
/// <remarks>
/// MetadataCatalog = describes one value set
/// Registry = knows all value sets
/// </remarks>
public interface IValueSetRegistry
{
    IReadOnlyCollection<ValueSetRegistration> Registrations { get; }
    ValueSetRegistration? Find(string valueSetKey);
}