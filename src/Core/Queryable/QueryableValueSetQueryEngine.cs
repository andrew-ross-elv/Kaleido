using Kaleido.Metadata;
using Kaleido.Validation;

namespace Kaleido.Queryable;

public sealed class QueryableValueSetQueryEngine<TRecord> : IValueSetQueryEngine<TRecord>
    where TRecord : class
{
    private readonly IValueSetMetadataCatalog _metadataCatalog;
    private readonly IValueSetQueryValidator _validator;
    private readonly IValueSetQueryCompiler _compiler;
    private readonly IQueryableValueSetSource<TRecord> _source;
    private readonly IEnumerable<IQueryableValueSetNamedQuery<TRecord>> _namedQueries;
    private readonly IQueryableCompiledQueryApplier<TRecord> _applier;
    private readonly IQueryableValueSetExecutor<TRecord> _executor;
    private readonly IServiceProvider? _services;

    public QueryableValueSetQueryEngine(
        IValueSetMetadataCatalog metadataCatalog,
        IValueSetQueryValidator validator,
        IValueSetQueryCompiler compiler,
        IQueryableValueSetSource<TRecord> source,
        IEnumerable<IQueryableValueSetNamedQuery<TRecord>> namedQueries,
        IQueryableCompiledQueryApplier<TRecord> applier,
        IQueryableValueSetExecutor<TRecord> executor,
        IServiceProvider? services = null)
    {
        _metadataCatalog = metadataCatalog;
        _validator = validator;
        _compiler = compiler;
        _source = source;
        _namedQueries = namedQueries;
        _applier = applier;
        _executor = executor;
        _services = services;
    }

    public async Task<QueryResult<TRecord>> ExecuteAsync(QueryRequest request, CancellationToken cancellationToken = default)
    {
        var metadata = _metadataCatalog.GetMetadata<TRecord>();
        _validator.Validate(request, metadata);
        var compiled = _compiler.Compile(request, metadata);
        var query = _source.CreateQuery(new ValueSetExecutionContext(metadata, request, _services));
        query = ApplyNamedQuery(query, compiled, metadata);
        query = _applier.ApplyFilter(query, compiled.Filter);
        query = _applier.ApplySearch(query, compiled.Search);
        query = _applier.ApplySort(query, compiled.Sort);
        var totalCount = await _executor.CountAsync(query, cancellationToken);
        var nextCursor = compiled.Page.Offset + compiled.Page.Size < totalCount
            ? CursorCodec.EncodeOffset(compiled.Page.Offset + compiled.Page.Size)
            : null;
        query = _applier.ApplyPage(query, compiled.Page);
        var items = await _executor.ToListAsync(query, cancellationToken);
        return new QueryResult<TRecord>(items, totalCount, nextCursor, metadata);
    }

    private IQueryable<TRecord> ApplyNamedQuery(IQueryable<TRecord> query, CompiledValueSetQuery compiled, RuntimeValueSetMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(compiled.NamedQuery)) return query;
        var handler = _namedQueries.SingleOrDefault(x => string.Equals(x.Name, compiled.NamedQuery, StringComparison.OrdinalIgnoreCase));
        if (handler is null) throw new InvalidOperationException($"Named query '{compiled.NamedQuery}' is allowed by metadata but no handler is registered for value set '{metadata.Name}'.");
        return handler.Apply(query, compiled.Parameters);
    }
}
