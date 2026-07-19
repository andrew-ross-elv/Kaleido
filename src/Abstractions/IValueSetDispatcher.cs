namespace Kaleido.Abstractions
{
    public interface IValueSetDispatcher
    {
        Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default);
    }
}