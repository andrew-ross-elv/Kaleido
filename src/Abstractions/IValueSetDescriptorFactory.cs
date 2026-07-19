namespace Kaleido.Abstractions
{
    public interface IValueSetDescriptorFactory
    {
        ValueSetDescriptor Create(RuntimeValueSetMetadata metadata);
    }
}