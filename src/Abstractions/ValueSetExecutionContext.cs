using Kaleido.Metadata;

namespace Kaleido
{
    public sealed record ValueSetExecutionContext
    (
        RuntimeValueSetMetadata Metadata,
        QueryRequest Request
    );
}