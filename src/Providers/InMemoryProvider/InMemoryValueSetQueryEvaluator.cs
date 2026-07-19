//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using ValueSet.Abstractions;

//namespace ValueSet.Providers.InMemory
//{
//    public sealed class InMemoryValueSetQueryEvaluator<TRecord>
//        : IValueSetQueryEvaluator<TRecord>
//        where TRecord : class
//    {
//        public InMemoryValueSetQueryEvaluator()
//        {
//        }

//        public QueryResult<TRecord> Evaluate(
//            IEnumerable<TRecord> source,
//            QueryRequest request,
//            ValueSetMetadata metadata)
//        {
//            var query = source;

//            query = ApplyFilter(query, request.Query?.Filter);
//            query = ApplySearch(query, request.Query?.Search, metadata);
//            query = ApplySort(query, request.Query?.Sort);

//            var totalCount = query.Count();

//            var items = ApplyPage(
//                    query,
//                    request.Query?.Page,
//                    metadata,
//                    out var nextCursor)
//                .ToList();

//            return new QueryResult<TRecord>(
//                items,
//                totalCount,
//                nextCursor,
//                metadata);
//        }

//        private IEnumerable<TRecord> ApplyFilter(IEnumerable<TRecord> source, IFilterExpression? expression)
//        {
//            if (expression is null)
//                return source;

//            return source.Where(record => EvaluateFilterExpression(record, expression));
//        }

//        private bool EvaluateFilterExpression(TRecord record, IFilterExpression expression)
//        {
//            return expression switch
//            {
//                QueryFilter filter =>
//                    EvaluateFilter(record, filter),

//                QueryFilterGroup group =>
//                    group.Operator switch
//                    {
//                        LogicalOperator.And =>
//                            group.Expressions.All(x => EvaluateFilterExpression(record, x)),

//                        LogicalOperator.Or =>
//                            group.Expressions.Any(x => EvaluateFilterExpression(record, x)),

//                        _ => throw new NotSupportedException(
//                            $"Logical operator '{group.Operator}' is not supported.")
//                    },

//                _ => throw new NotSupportedException(
//                    $"Filter expression type '{expression.GetType().Name}' is not supported.")
//            };
//        }

//        private bool EvaluateFilter(TRecord record, QueryFilter filter)
//        {
//            var property = GetProperty(filter.Field);
//            var actual = property.GetValue(record);

//            return filter.Operator switch
//            {
//                FilterOperator.Eq =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) == 0,

//                FilterOperator.Ne =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) != 0,

//                FilterOperator.Gt =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) > 0,

//                FilterOperator.Gte =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) >= 0,

//                FilterOperator.Lt =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) < 0,

//                FilterOperator.Lte =>
//                    Compare(actual, filter.Values.Single(), property.PropertyType) <= 0,

//                FilterOperator.Contains =>
//                    ToStringValue(actual).Contains(
//                        ToStringValue(filter.Values.Single()),
//                        StringComparison.OrdinalIgnoreCase),

//                FilterOperator.NotContains =>
//                    !ToStringValue(actual).Contains(
//                        ToStringValue(filter.Values.Single()),
//                        StringComparison.OrdinalIgnoreCase),

//                FilterOperator.StartsWith =>
//                    ToStringValue(actual).StartsWith(
//                        ToStringValue(filter.Values.Single()),
//                        StringComparison.OrdinalIgnoreCase),

//                FilterOperator.EndsWith =>
//                    ToStringValue(actual).EndsWith(
//                        ToStringValue(filter.Values.Single()),
//                        StringComparison.OrdinalIgnoreCase),

//                FilterOperator.In =>
//                    filter.Values.Any(value =>
//                        Compare(actual, value, property.PropertyType) == 0),

//                FilterOperator.NotIn =>
//                    filter.Values.All(value =>
//                        Compare(actual, value, property.PropertyType) != 0),

//                FilterOperator.Between =>
//                    Compare(actual, filter.Values[0], property.PropertyType) >= 0
//                    && Compare(actual, filter.Values[1], property.PropertyType) <= 0,

//                FilterOperator.NotBetween =>
//                    Compare(actual, filter.Values[0], property.PropertyType) < 0
//                    || Compare(actual, filter.Values[1], property.PropertyType) > 0,

//                FilterOperator.IsNull =>
//                    actual is null,

//                FilterOperator.IsNotNull =>
//                    actual is not null,

//                FilterOperator.IsTrue =>
//                    Compare(actual, true, property.PropertyType) == 0,

//                FilterOperator.IsFalse =>
//                    Compare(actual, false, property.PropertyType) == 0,

//                _ => throw new NotSupportedException(
//                    $"Filter operator '{filter.Operator}' is not supported.")
//            };
//        }

//        private IEnumerable<TRecord> ApplySearch(IEnumerable<TRecord> source, ISearchExpression? expression, ValueSetMetadata metadata)
//        {
//            if (expression is null)
//                return source;

//            return source.Where(record => EvaluateSearchExpression(record, expression, metadata));
//        }

//        private bool EvaluateSearchExpression(TRecord record, ISearchExpression expression, ValueSetMetadata metadata)
//        {
//            return expression switch
//            {
//                QuerySearch search =>
//                    EvaluateSearch(record, search, metadata),

//                QuerySearchGroup group =>
//                    group.Operator switch
//                    {
//                        LogicalOperator.And =>
//                            group.Expressions.All(x => EvaluateSearchExpression(record, x, metadata)),

//                        LogicalOperator.Or =>
//                            group.Expressions.Any(x => EvaluateSearchExpression(record, x, metadata)),

//                        _ => throw new NotSupportedException(
//                            $"Logical operator '{group.Operator}' is not supported.")
//                    },

//                _ => throw new NotSupportedException(
//                    $"Search expression type '{expression.GetType().Name}' is not supported.")
//            };
//        }

//        private bool EvaluateSearch(TRecord record, QuerySearch search, ValueSetMetadata metadata)
//        {
//            var searchableFields = metadata.Fields
//                .Where(x => x.IsSearchable)
//                .Where(x => x.MatchModes.Contains(search.MatchMode))
//                .Where(x => search.Field is null
//                    || string.Equals(x.Name, search.Field, StringComparison.OrdinalIgnoreCase))
//                .OrderBy(x => x.SearchPriority ?? int.MaxValue)
//                .ToList();

//            if (searchableFields.Count == 0)
//                return false;

//            foreach (var field in searchableFields)
//            {
//                var property = GetProperty(field.Name);
//                var actual = ToStringValue(property.GetValue(record));

//                if (Matches(actual, search.SearchText, search.MatchMode))
//                    return true;
//            }

//            return false;
//        }

//        private static bool Matches(string actual, string searchText, MatchMode matchMode)
//        {
//            return matchMode switch
//            {
//                MatchMode.Exact =>
//                    string.Equals(actual, searchText, StringComparison.OrdinalIgnoreCase),

//                MatchMode.StartsWith =>
//                    actual.StartsWith(searchText, StringComparison.OrdinalIgnoreCase),

//                MatchMode.EndsWith =>
//                    actual.EndsWith(searchText, StringComparison.OrdinalIgnoreCase),

//                MatchMode.Contains =>
//                    actual.Contains(searchText, StringComparison.OrdinalIgnoreCase),

//                _ => throw new NotSupportedException(
//                    $"Match mode '{matchMode}' is not supported by in-memory provider.")
//            };
//        }

//        private IEnumerable<TRecord> ApplySort(IEnumerable<TRecord> source, IReadOnlyList<QuerySort>? sorts)
//        {
//            if (sorts is null || sorts.Count == 0)
//                return source;

//            IOrderedEnumerable<TRecord>? ordered = null;

//            foreach (var sort in sorts.OrderBy(x => x.Sequence ?? int.MaxValue))
//            {
//                var property = GetProperty(sort.Field);

//                Func<TRecord, object?> keySelector = record =>
//                    property.GetValue(record);

//                ordered = ordered is null
//                    ? sort.Direction == SortDirection.Asc
//                        ? source.OrderBy(keySelector)
//                        : source.OrderByDescending(keySelector)
//                    : sort.Direction == SortDirection.Asc
//                        ? ordered.ThenBy(keySelector)
//                        : ordered.ThenByDescending(keySelector);
//            }

//            return ordered ?? source;
//        }

//        private IEnumerable<TRecord> ApplyPage(IEnumerable<TRecord> source, QueryPage? page, ValueSetMetadata metadata, out string? nextCursor)
//        {
//            var pageMetadata = metadata.Pageable;

//            var defaultSize = pageMetadata?.DefaultSize ?? 50;
//            var maxSize = pageMetadata?.MaxSize ?? 100;

//            var size = page?.Size ?? defaultSize;

//            if (size <= 0)
//                throw new InvalidOperationException("Page size must be greater than zero.");

//            if (size > maxSize)
//                throw new InvalidOperationException(
//                    $"Page size '{size}' exceeds max page size '{maxSize}'.");

//            var offset = DecodeCursor(page?.Cursor);

//            var items = source
//                .Skip(offset)
//                .Take(size + 1)
//                .ToList();

//            var hasMore = items.Count > size;

//            nextCursor = hasMore
//                ? EncodeCursor(offset + size)
//                : null;

//            return items.Take(size);
//        }

//        private PropertyInfo GetProperty(string field)
//        {
//            return typeof(TRecord).GetProperties()
//                .Single(x => string.Equals(x.Name, field, StringComparison.OrdinalIgnoreCase));
//        }

//        private static string ToStringValue(object? value)
//        {
//            return value?.ToString() ?? string.Empty;
//        }

//        private static int Compare(
//            object? actual,
//            object? expected,
//            Type targetType)
//        {
//            if (actual is null && expected is null)
//                return 0;

//            if (actual is null)
//                return -1;

//            if (expected is null)
//                return 1;

//            var convertedExpected = ConvertValue(expected, targetType);

//            if (actual is IComparable comparable)
//                return comparable.CompareTo(convertedExpected);

//            throw new InvalidOperationException(
//                $"Type '{targetType.Name}' does not implement IComparable.");
//        }

//        private static object? ConvertValue(
//            object? value,
//            Type targetType)
//        {
//            if (value is null)
//                return null;

//            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

//            if (underlying.IsEnum)
//                return Enum.Parse(underlying, value.ToString()!, ignoreCase: true);

//            if (underlying == typeof(Guid))
//                return Guid.Parse(value.ToString()!);

//            return Convert.ChangeType(value, underlying);
//        }

//        private static string EncodeCursor(int offset)
//        {
//            return Convert.ToBase64String(
//                Encoding.UTF8.GetBytes(offset.ToString()));
//        }

//        private static int DecodeCursor(string? cursor)
//        {
//            if (string.IsNullOrWhiteSpace(cursor))
//                return 0;

//            var text = Encoding.UTF8.GetString(
//                Convert.FromBase64String(cursor));

//            return int.Parse(text);
//        }

//    }
//}