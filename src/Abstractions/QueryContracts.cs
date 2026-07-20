using Kaleido.Metadata;

namespace Kaleido;

/// <summary>Value-set query request.</summary>
public record QueryRequest
(
    string? QueryName,
    QueryBody? Query,
    IReadOnlyDictionary<string, object?>? Parameters = null
);

//public record QueryContext
//(
//    string? BusinessUnit,
//    string? Solution,
//    string? SubSolution,
//    string? Product,
//    string? ClientId,
//    string? LineOfBusiness,
//    DateTime? DateOfService,
//    DateTime? AsOfDate,
//    string? WorkflowContextId,
//    string? MemberContextId
//);

/// <summary>Filter/search/sort/page section of a query request.</summary>
public record QueryBody
(
    ISearchExpression? Search,
    IFilterExpression? Filter,
    IReadOnlyList<QuerySort>? Sort,
    QueryPage? Page
);

public interface IFilterExpression { }
public record QueryFilterGroup(LogicalOperator Operator, List<IFilterExpression> Expressions) : IFilterExpression;
public record QueryFilter(string Field, FilterOperator Operator, List<object?>? Values) : IFilterExpression;


public interface ISearchExpression { }
public record QuerySearch(string SearchText, MatchMode MatchMode, string? Field = null) : ISearchExpression;
public record QuerySearchGroup(LogicalOperator Operator, List<ISearchExpression> Expressions) : ISearchExpression;

public record QuerySort(string Field, SortDirection Direction, int? Sequence = null);
public record QueryPage(int? Size, string? Cursor = null);

public interface IValueSetQueryResult
{
    Type RecordType { get; }
    int TotalCount { get; }
    string? NextCursor { get; }
    RuntimeValueSetMetadata RuntimeMetadata { get; }
    IReadOnlyList<object> ItemsAsObjects { get; }
}

public sealed record QueryResult<TRecord>(IReadOnlyList<TRecord> Items, int TotalCount, string? NextCursor, RuntimeValueSetMetadata RuntimeMetadata)
    : IValueSetQueryResult
    where TRecord : class
{
    public Type RecordType => typeof(TRecord);
    public IReadOnlyList<object> ItemsAsObjects => Items.Cast<object>().ToArray();
}

public sealed record ValueSetQueryResponse(
    ValueSetDescriptor ValueSet,
    int TotalCount,
    string? NextCursor,
    IReadOnlyList<object> Items);
