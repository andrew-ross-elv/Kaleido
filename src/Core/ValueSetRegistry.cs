using Kaleido.Abstractions;

namespace Kaleido.Core;

public sealed class ValueSetRegistry : IValueSetRegistry
{
    private readonly IReadOnlyDictionary<string, ValueSetRegistration> _registrations;

    public ValueSetRegistry(IEnumerable<ValueSetRegistration> registrations)
    {
        _registrations = registrations
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<ValueSetRegistration> Registrations => _registrations.Values.ToArray();
    public ValueSetRegistration? Find(string valueSetKey) => _registrations.TryGetValue(valueSetKey, out var r) ? r : null;
}
