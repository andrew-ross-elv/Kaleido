namespace Kaleido.Abstractions
{
    public sealed record ValueSetExecutionContext
    (
        RuntimeValueSetMetadata Metadata,
        QueryRequest Request,
        IServiceProvider Services
    );
}