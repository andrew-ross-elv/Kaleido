using System.Reflection;
using Kaleido.Abstractions;
using Kaleido.Abstractions.Attributes;

namespace Kaleido.Core;

public static class ValueSetMetadataBuilder
{
    public static RuntimeValueSetMetadata Build<TRecord>() where TRecord : class => Build(typeof(TRecord));

    public static RuntimeValueSetMetadata Build(Type recordType)
    {
        var valueSet = recordType.GetCustomAttribute<ValueSetAttribute>()
            ?? throw new InvalidOperationException($"{recordType.Name} is missing ValueSetAttribute.");
        var pageable = recordType.GetCustomAttribute<PageableAttribute>();
        var fields = recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(BuildField).ToArray();
        var allowed = recordType.GetCustomAttributes<AllowedQueryAttribute>()
            .Select(x => new RuntimeAllowedQueryMetadata(x.Name, x.Description, x.Parameters)).ToArray();

        return new RuntimeValueSetMetadata(valueSet.Name, valueSet.Version, valueSet.Source, fields, allowed,
            pageable is null ? null : new RuntimePageableMetadata(pageable.DefaultSize, pageable.MaxSize, pageable.CursorSupported));
    }

    private static RuntimeFieldMetadata BuildField(PropertyInfo property)
    {
        var filterable = property.GetCustomAttribute<FilterableAttribute>();
        var searchable = property.GetCustomAttribute<SearchableAttribute>();
        var sortable = property.GetCustomAttribute<SortableAttribute>();

        return new RuntimeFieldMetadata(
            property.Name,
            property.PropertyType,
            filterable is not null,
            filterable?.Operators ?? Array.Empty<FilterOperator>(),
            searchable is not null,
            searchable?.Priority,
            searchable?.MatchModes ?? Array.Empty<MatchMode>(),
            sortable is not null,
            sortable?.Directions ?? Array.Empty<SortDirection>());
    }
}
