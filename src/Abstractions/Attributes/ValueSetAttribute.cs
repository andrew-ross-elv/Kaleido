namespace Kaleido.Attributes;

/// <summary>Marks a record type as a framework-discoverable value set.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValueSetAttribute : Attribute
{
    /// <summary>Creates value-set metadata for a record type.</summary>
    /// <param name="name">Logical value-set key used for lookup and query execution.</param>
    /// <param name="version">Version of the value-set metadata.</param>
    /// <param name="source">Name of the authoritative source for the value set.</param>
    public ValueSetAttribute(string name, string version, string source)
    {
        Name = name;
        Version = version;
        Source = source;
    }

    public string Name { get; }
    public string Version { get; }
    public string Source { get; }
}
