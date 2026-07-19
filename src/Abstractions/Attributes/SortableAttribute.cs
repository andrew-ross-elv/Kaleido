namespace Kaleido.Abstractions.Attributes;

/// <summary>Declares a property as sortable.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SortableAttribute : Attribute
{
    /// <summary>Creates sort metadata for a property.</summary>
    /// <param name="directions">Allowed directions. If omitted, both asc and desc are allowed.</param>
    public SortableAttribute(params SortDirection[] directions)
    {
        Directions = directions.Length == 0 ? new[] { SortDirection.Asc, SortDirection.Desc } : directions;
    }

    public IReadOnlyList<SortDirection> Directions { get; }
}
