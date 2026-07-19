namespace Kaleido.Abstractions;

/// <summary>Consumer-safe metadata for a registered value set.</summary>
public sealed record ValueSetDescriptor(
    string Name,
    string Version,
    string Source,
    IReadOnlyList<FieldDescriptor> Fields,
    IReadOnlyList<AllowedQueryDescriptor> AllowedQueries,
    PageableDescriptor? Pageable);

/// <summary>Consumer-safe metadata for a value-set field.</summary>
public sealed record FieldDescriptor(
    string Name,
    DataTypeDescriptor DataType,
    bool IsFilterable,
    IReadOnlyList<string> FilterOperators,
    bool IsSearchable,
    IReadOnlyList<string> MatchModes,
    bool IsSortable,
    IReadOnlyList<string> SortDirections);

/// <summary>OpenAPI/JSON-Schema-style data type shape.</summary>
public sealed record DataTypeDescriptor(string Type, string? Format = null);

/// <summary>Consumer-safe allowed named-query metadata.</summary>
public sealed record AllowedQueryDescriptor(string Name, string Description, IReadOnlyList<string> Parameters);

/// <summary>Consumer-safe paging metadata.</summary>
public sealed record PageableDescriptor(int DefaultSize, int MaxSize, bool CursorSupported);
