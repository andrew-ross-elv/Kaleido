using Kaleido.Abstractions;

namespace Kaleido.Core;

public sealed class ValueSetQueryCompiler : IValueSetQueryCompiler
{
    public CompiledValueSetQuery Compile(QueryRequest request, RuntimeValueSetMetadata metadata)
    {
        var pageable = metadata.Pageable;
        var size = request.Query?.Page?.Size ?? pageable?.DefaultSize ?? 50;
        var offset = CursorCodec.DecodeOffset(request.Query?.Page?.Cursor);
        return new CompiledValueSetQuery(
            request.QueryName,
            request.Parameters,
            CompileFilter(request.Query?.Filter, metadata),
            CompileSearch(request.Query?.Search, metadata),
            CompileSort(request.Query?.Sort, metadata),
            new CompiledPage(size, offset, pageable?.CursorSupported ?? false));
    }

    private static CompiledFilterExpression? CompileFilter(IFilterExpression? expression, RuntimeValueSetMetadata metadata)
    {
        return expression switch
        {
            null => null,
            QueryFilter f => new CompiledFilterCondition(GetField(metadata, f.Field), f.Operator, f.Values),
            QueryFilterGroup g => new CompiledFilterGroup(g.Operator, g.Expressions.Select(x => CompileFilter(x, metadata)!).ToArray()),
            _ => throw new NotSupportedException($"Unsupported filter expression type '{expression.GetType().Name}'.")
        };
    }

    private static CompiledSearchExpression? CompileSearch(ISearchExpression? expression, RuntimeValueSetMetadata metadata)
    {
        return expression switch
        {
            null => null,
            QuerySearch s => CompileSearchCondition(s, metadata),
            QuerySearchGroup g => new CompiledSearchGroup(g.Operator, g.Expressions.Select(x => CompileSearch(x, metadata)!).ToArray()),
            _ => throw new NotSupportedException($"Unsupported search expression type '{expression.GetType().Name}'.")
        };
    }

    private static CompiledSearchExpression CompileSearchCondition(QuerySearch search, RuntimeValueSetMetadata metadata)
    {
        var conditions = metadata.Fields
            .Where(x => x.IsSearchable)
            .Where(x => x.MatchModes.Contains(search.MatchMode))
            .Where(x => search.Field is null || string.Equals(x.Name, search.Field, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.SearchPriority ?? int.MaxValue)
            .Select(x => (CompiledSearchExpression)new CompiledSearchCondition(x, search.SearchText, search.MatchMode))
            .ToArray();
        return conditions.Length == 1 ? conditions[0] : new CompiledSearchGroup(LogicalOperator.Or, conditions);
    }

    private static IReadOnlyList<CompiledSort> CompileSort(IReadOnlyList<QuerySort>? sorts, RuntimeValueSetMetadata metadata)
    {
        if (sorts is null || sorts.Count == 0) return Array.Empty<CompiledSort>();
        return sorts.OrderBy(x => x.Sequence ?? int.MaxValue).Select((x, index) => new CompiledSort(GetField(metadata, x.Field), x.Direction, index)).ToArray();
    }

    private static RuntimeFieldMetadata GetField(RuntimeValueSetMetadata metadata, string fieldName)
    {
        return metadata.Fields.Single(x => string.Equals(x.Name, fieldName, StringComparison.OrdinalIgnoreCase));
    }
}
