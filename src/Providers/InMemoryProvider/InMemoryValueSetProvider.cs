//using ValueSet.Abstractions;

//namespace ValueSet.Providers.InMemory;

//public sealed class InMemoryValueSetProvider<TRecord> : IValueSetProvider<TRecord>
//    where TRecord : class
//{
//    private readonly IValueSetDataSource<TRecord> _dataSource;
//    private readonly IReadOnlyDictionary<string, IValueSetNamedQuery<TRecord>> _namedQueries;
//    private readonly IValueSetQueryEvaluator<TRecord> _queryEvaluator;
//    private readonly IValueSetQueryValidator _validator;

//    public InMemoryValueSetProvider(
//        IValueSetMetadataCatalog metadataCatalog,
//        IValueSetDataSource<TRecord> dataSource,
//        IEnumerable<IValueSetNamedQuery<TRecord>> namedQueries,
//        IValueSetQueryEvaluator<TRecord> queryEvaluator,
//        IValueSetQueryValidator validator)
//    {
//        ArgumentNullException.ThrowIfNull(metadataCatalog); 
//        ArgumentNullException.ThrowIfNull(dataSource); 
//        ArgumentNullException.ThrowIfNull(validator); 
//        ArgumentNullException.ThrowIfNull(queryEvaluator); 
//        ArgumentNullException.ThrowIfNull(namedQueries); 
        
//        _dataSource = dataSource;
//        _queryEvaluator = queryEvaluator;
//        _validator = validator;
//        Metadata = metadataCatalog.GetMetadata<TRecord>();
//        _namedQueries = namedQueries.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
//    }

//    public ValueSetMetadata Metadata { get; }

//    public async Task<QueryResult<TRecord>> QueryAsync(
//        QueryRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        cancellationToken.ThrowIfCancellationRequested();
//        ArgumentNullException.ThrowIfNull(request);

//        _validator.Validate(request, Metadata);

//        var query = await _dataSource.GetRecordsAsync(cancellationToken);

//        query = ApplyNamedQuery(query, request);

//        return _queryEvaluator.Evaluate(query, request, Metadata);
//    }

//    private IEnumerable<TRecord> ApplyNamedQuery(
//        IEnumerable<TRecord> source,
//        QueryRequest request)
//    {
//        if (string.IsNullOrWhiteSpace(request.QueryName))
//            return source;

//        if (!_namedQueries.TryGetValue(request.QueryName, out var namedQuery))
//            throw new InvalidOperationException(
//                $"Named query '{request.QueryName}' is not registered for value set '{Metadata.Name}'.");

//        return namedQuery.Apply(source, request.Parameters);
//    }



//}