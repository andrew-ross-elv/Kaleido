//using ValueSet.Abstractions;

//namespace ValueSet.InMemory;

//public sealed class InMemoryValueSetSource<TRecord> : IQueryableValueSetSource<TRecord>
//    where TRecord : class
//{
//    private readonly IReadOnlyList<TRecord> _records;
//    public InMemoryValueSetSource(IEnumerable<TRecord> records) => _records = records.ToArray();
//    public IQueryable<TRecord> CreateQuery(ValueSetExecutionContext context) => _records.AsQueryable();
//}
