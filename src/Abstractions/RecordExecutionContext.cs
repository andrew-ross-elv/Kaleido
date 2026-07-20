using Kaleido.Metadata;

namespace Kaleido
{
    public sealed record RecordExecutionContext
    (
        RuntimeRecordMetadata Metadata,
        KaleidoQueryRequest Request
    );
}