using Kaleido.Metadata;
using Kaleido.Registry;

namespace Kaleido;

public sealed class RecordRegistry : IRecordRegistry
{
    private readonly IReadOnlyDictionary<string, RecordRegistration> _registrations;

    public RecordRegistry(IEnumerable<RecordRegistration> registrations)
    {
        _registrations = registrations
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<RecordRegistration> Registrations => _registrations.Values.ToArray();
    public RecordRegistration? Find(string recordKey) => _registrations.TryGetValue(recordKey, out var r) ? r : null;
}
