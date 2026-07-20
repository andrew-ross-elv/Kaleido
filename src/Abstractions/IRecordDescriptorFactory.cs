using Kaleido.Metadata;

namespace Kaleido
{
    public interface IRecordDescriptorFactory
    {
        RecordDescriptor Create(RuntimeRecordMetadata metadata);
    }
}