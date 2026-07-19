//using ValueSet.Abstractions;

//namespace ValueSet.Queryable;

//public sealed class CompiledValueSetQueryEngine<TRecord> : IValueSetQueryEngine<TRecord>
//    where TRecord : class
//{
//    private readonly IValueSetMetadataCatalog _metadataCatalog;
//    private readonly IValueSetQueryValidator _validator;
//    private readonly IValueSetQueryCompiler _compiler;
//    private readonly ICompiledValueSetExecutor<TRecord> _executor;

//    public CompiledValueSetQueryEngine(
//        IValueSetMetadataCatalog metadataCatalog,
//        IValueSetQueryValidator validator,
//        IValueSetQueryCompiler compiler,
//        ICompiledValueSetExecutor<TRecord> executor)
//    {
//        _metadataCatalog = metadataCatalog;
//        _validator = validator;
//        _compiler = compiler;
//        _executor = executor;
//    }

//    public Task<QueryResult<TRecord>> ExecuteAsync(QueryRequest request, CancellationToken cancellationToken = default)
//    {
//        var metadata = _metadataCatalog.GetMetadata<TRecord>();
//        _validator.Validate(request, metadata);
//        var compiled = _compiler.Compile(request, metadata);
//        return _executor.ExecuteAsync(compiled, metadata, cancellationToken);
//    }
//}
