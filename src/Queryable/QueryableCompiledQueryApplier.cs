using System.Linq.Expressions;
using System.Reflection;
using Kaleido.Abstractions;

namespace Kaleido.Queryable;

public sealed class QueryableCompiledQueryApplier<TRecord> : IQueryableCompiledQueryApplier<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> ApplyFilter(IQueryable<TRecord> query, CompiledFilterExpression? filter)
    {
        if (filter is null) return query;
        var p = Expression.Parameter(typeof(TRecord), "x");
        return query.Where(Expression.Lambda<Func<TRecord, bool>>(BuildFilter(p, filter), p));
    }

    public IQueryable<TRecord> ApplySearch(IQueryable<TRecord> query, CompiledSearchExpression? search)
    {
        if (search is null) return query;
        var p = Expression.Parameter(typeof(TRecord), "x");
        return query.Where(Expression.Lambda<Func<TRecord, bool>>(BuildSearch(p, search), p));
    }

    public IQueryable<TRecord> ApplySort(IQueryable<TRecord> query, IReadOnlyList<CompiledSort> sort)
    {
        var ordered = false;
        foreach (var s in sort.OrderBy(x => x.Sequence))
        {
            query = ApplySortItem(query, s, ordered);
            ordered = true;
        }
        return query;
    }

    public IQueryable<TRecord> ApplyPage(IQueryable<TRecord> query, CompiledPage page) => query.Skip(page.Offset).Take(page.Size);

    private static Expression BuildFilter(ParameterExpression p, CompiledFilterExpression e) => e switch
    {
        CompiledFilterCondition c => BuildFilterCondition(p, c),
        CompiledFilterGroup g => BuildGroup(g.Operator, g.Expressions.Select(x => BuildFilter(p, x)).ToArray()),
        _ => throw new NotSupportedException($"Unsupported compiled filter type '{e.GetType().Name}'.")
    };

    private static Expression BuildSearch(ParameterExpression p, CompiledSearchExpression e) => e switch
    {
        CompiledSearchCondition c => BuildSearchCondition(p, c),
        CompiledSearchGroup g => BuildGroup(g.Operator, g.Expressions.Select(x => BuildSearch(p, x)).ToArray()),
        _ => throw new NotSupportedException($"Unsupported compiled search type '{e.GetType().Name}'.")
    };

    private static Expression BuildGroup(LogicalOperator op, IReadOnlyList<Expression> expressions)
    {
        if (expressions.Count == 0) return Expression.Constant(true);
        return op == LogicalOperator.And ? expressions.Aggregate(Expression.AndAlso) : expressions.Aggregate(Expression.OrElse);
    }

    private static Expression BuildFilterCondition(ParameterExpression p, CompiledFilterCondition c)
    {
        var member = Expression.PropertyOrField(p, c.Field.Name);
        return c.Operator switch
        {
            FilterOperator.Eq => Expression.Equal(member, Constant(member.Type, c.Values[0])),
            FilterOperator.Ne => Expression.NotEqual(member, Constant(member.Type, c.Values[0])),
            FilterOperator.Contains => StringCall(member, nameof(string.Contains), c.Values[0]),
            FilterOperator.StartsWith => StringCall(member, nameof(string.StartsWith), c.Values[0]),
            FilterOperator.In => InCall(member, c.Values, false),
            _ => throw new NotSupportedException($"Operator '{c.Operator}' shortened in prototype for readability.")
        };
    }

    private static Expression BuildSearchCondition(ParameterExpression p, CompiledSearchCondition c)
    {
        var member = Expression.PropertyOrField(p, c.Field.Name);
        return c.MatchMode switch
        {
            MatchMode.Exact => Expression.Equal(member, Constant(member.Type, c.SearchText)),
            MatchMode.StartsWith => StringCall(member, nameof(string.StartsWith), c.SearchText),
            MatchMode.Contains => StringCall(member, nameof(string.Contains), c.SearchText),
            _ => throw new NotSupportedException($"Match mode '{c.MatchMode}' shortened in prototype for readability.")
        };
    }

    private static IQueryable<TRecord> ApplySortItem(IQueryable<TRecord> query, CompiledSort sort, bool thenBy)
    {
        var p = Expression.Parameter(typeof(TRecord), "x");
        var member = Expression.PropertyOrField(p, sort.Field.Name);
        var lambda = Expression.Lambda(member, p);
        var methodName = (thenBy, sort.Direction) switch
        {
            (false, SortDirection.Asc) => nameof(global::System.Linq.Queryable.OrderBy),
            (false, SortDirection.Desc) => nameof(global::System.Linq.Queryable.OrderByDescending),
            (true, SortDirection.Asc) => nameof(global::System.Linq.Queryable.ThenBy),
            (true, SortDirection.Desc) => nameof(global::System.Linq.Queryable.ThenByDescending),
            _ => throw new NotSupportedException()
        };
        var method = typeof(System.Linq.Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
            .Single()
            .MakeGenericMethod(typeof(TRecord), member.Type);
        return (IQueryable<TRecord>)method.Invoke(null, new object[] { query, lambda })!;
    }

    private static Expression StringCall(Expression member, string methodName, object? value)
    {
        var method = typeof(string).GetMethod(methodName, new[] { typeof(string) })!;
        return Expression.Call(member, method, Expression.Constant(value?.ToString() ?? string.Empty));
    }

    private static Expression InCall(Expression member, IReadOnlyList<object?> values, bool negate)
    {
        var typedArray = Array.CreateInstance(member.Type, values.Count);
        for (var i = 0; i < values.Count; i++) typedArray.SetValue(ConvertValue(values[i], member.Type), i);
        var method = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
            .MakeGenericMethod(member.Type);
        var call = Expression.Call(method, Expression.Constant(typedArray), member);
        return negate ? Expression.Not(call) : call;
    }

    private static ConstantExpression Constant(Type targetType, object? value) => Expression.Constant(ConvertValue(value, targetType), targetType);

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null) return null;
        var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (t.IsEnum) return Enum.Parse(t, value.ToString()!, true);
        if (t == typeof(Guid)) return Guid.Parse(value.ToString()!);
        return Convert.ChangeType(value, t);
    }
}
