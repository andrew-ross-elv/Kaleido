using Kaleido.Abstractions;

namespace Kaleido.Abstractions.Attributes;

/// <summary>Declares a property as searchable.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SearchableAttribute : Attribute
{
    /// <summary>Creates search metadata for a property.</summary>
    /// <param name="priority">Priority used when a search applies to multiple fields.</param>
    /// <param name="matchModes">Search match modes supported by this property.</param>
    public SearchableAttribute(int priority, params MatchMode[] matchModes)
    {
        Priority = priority;
        MatchModes = matchModes;
    }

    public int Priority { get; }
    public IReadOnlyList<MatchMode> MatchModes { get; }
}
