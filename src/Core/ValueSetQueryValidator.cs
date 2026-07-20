using Kaleido.Metadata;
using Kaleido.Validation;

namespace Kaleido;

public sealed class ValueSetQueryValidator : IValueSetQueryValidator
{
    public void Validate(QueryRequest request, RuntimeValueSetMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateNamedQuery(request, metadata);
        ValidateFilter(request.Query?.Filter, metadata);
        ValidateSearch(request.Query?.Search, metadata);
        ValidateSort(request.Query?.Sort, metadata);
        ValidatePage(request.Query?.Page, metadata);
    }

    private static void ValidateNamedQuery(QueryRequest request, RuntimeValueSetMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(request.QueryName)) return;
        var allowed = metadata.AllowedQueries.SingleOrDefault(x => string.Equals(x.Name, request.QueryName, StringComparison.OrdinalIgnoreCase));
        if (allowed is null) throw new InvalidOperationException($"Named query '{request.QueryName}' is not allowed for value set '{metadata.Name}'.");
        foreach (var p in allowed.Parameters)
        {
            if (request.Parameters is null || !request.Parameters.ContainsKey(p) || request.Parameters[p] is null)
                throw new InvalidOperationException($"Named query '{request.QueryName}' requires parameter '{p}'.");
        }
    }

    private static void ValidateFilter(IFilterExpression? expression, RuntimeValueSetMetadata metadata)
    {
        if (expression is null) return;
        switch (expression)
        {
            case QueryFilter f: ValidateFilterCondition(f, metadata); return;
            case QueryFilterGroup g:
                if (g.Expressions.Count == 0) throw new InvalidOperationException("Filter group must contain at least one expression.");
                foreach (var child in g.Expressions) ValidateFilter(child, metadata);
                return;
            default: throw new InvalidOperationException($"Unsupported filter expression type '{expression.GetType().Name}'.");
        }
    }

    private static void ValidateFilterCondition(QueryFilter filter, RuntimeValueSetMetadata metadata)
    {
        var field = GetField(metadata, filter.Field);
        if (!field.IsFilterable) throw new InvalidOperationException($"Field '{filter.Field}' is not filterable.");
        if (!field.FilterOperators.Contains(filter.Operator)) throw new InvalidOperationException($"Operator '{filter.Operator}' is not supported for field '{filter.Field}'.");
    }

    private static void ValidateSearch(ISearchExpression? expression, RuntimeValueSetMetadata metadata)
    {
        if (expression is null) return;
        switch (expression)
        {
            case QuerySearch s: ValidateSearchCondition(s, metadata); return;
            case QuerySearchGroup g:
                foreach (var child in g.Expressions) ValidateSearch(child, metadata);
                return;
            default: throw new InvalidOperationException($"Unsupported search expression type '{expression.GetType().Name}'.");
        }
    }

    private static void ValidateSearchCondition(QuerySearch search, RuntimeValueSetMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(search.SearchText)) throw new InvalidOperationException("Search text is required.");
        var fields = metadata.Fields.Where(x => x.IsSearchable);
        if (!string.IsNullOrWhiteSpace(search.Field)) fields = fields.Where(x => string.Equals(x.Name, search.Field, StringComparison.OrdinalIgnoreCase));
        var list = fields.ToArray();
        if (list.Length == 0) throw new InvalidOperationException($"No searchable fields exist for search field '{search.Field ?? "*"}'.");
        if (!list.Any(x => x.MatchModes.Contains(search.MatchMode))) throw new InvalidOperationException($"Match mode '{search.MatchMode}' is not supported for search field '{search.Field ?? "*"}'.");
    }

    private static void ValidateSort(IReadOnlyList<QuerySort>? sorts, RuntimeValueSetMetadata metadata)
    {
        if (sorts is null) return;
        foreach (var sort in sorts)
        {
            var field = GetField(metadata, sort.Field);
            if (!field.IsSortable) throw new InvalidOperationException($"Field '{sort.Field}' is not sortable.");
        }
    }

    private static void ValidatePage(QueryPage? page, RuntimeValueSetMetadata metadata)
    {
        if (page is null) return;
        var pageable = metadata.Pageable ?? throw new InvalidOperationException($"Value set '{metadata.Name}' does not support paging.");
        if (page.Size is <= 0) throw new InvalidOperationException("Page size must be greater than zero.");
        if (page.Size.HasValue && page.Size.Value > pageable.MaxSize) throw new InvalidOperationException($"Page size '{page.Size.Value}' exceeds max page size '{pageable.MaxSize}'.");
    }

    private static RuntimeFieldMetadata GetField(RuntimeValueSetMetadata metadata, string name)
    {
        return metadata.Fields.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Field '{name}' does not exist on value set '{metadata.Name}'.");
    }
}
