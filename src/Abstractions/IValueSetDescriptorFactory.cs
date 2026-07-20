using Kaleido.Metadata;

namespace Kaleido
{
    public interface IValueSetDescriptorFactory
    {
        ValueSetDescriptor Create(RuntimeValueSetMetadata metadata);
    }
}