namespace Kaleido.Abstractions
{
    /// <summary>Primary consumer-facing facade for metadata discovery and query execution.</summary>
    public interface IValueSetCatalog
    {
        /// <summary>Returns descriptors for all registered value sets.</summary>
        /// <returns>Registered value-set descriptors.</returns>
        IReadOnlyCollection<ValueSetDescriptor> GetAll();

        /// <summary>Returns one registered value-set descriptor.</summary>
        /// <param name="valueSetKey">Logical key defined by <see cref="ValueSetAttribute"/>.</param>
        /// <returns>The descriptor, or null when the value set is not registered.</returns>
        ValueSetDescriptor? Get(string valueSetKey);

        /// <summary>Executes a query against a registered value set.</summary>
        /// <param name="valueSetKey">Logical key defined by <see cref="ValueSetAttribute"/>.</param>
        /// <param name="request">Query request containing optional named query, filters, search, sort, and paging.</param>
        /// <param name="cancellationToken">Token used to cancel query execution.</param>
        /// <returns>Query response containing metadata and matching items.</returns>
        Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default);
    }
}