using Kaleido.Abstractions;
using Kaleido.Abstractions.Attributes;
using Kaleido.Core;
using Xunit;

namespace Core.Tests
{
    public class ValueSetMetadataCatalogTests
    {
        [Fact]
        public void GetMetadata_Caches_ByType()
        {
            var catalog = new ValueSetMetadataCatalog();
            var a = catalog.GetMetadata(typeof(TestRecord));
            var b = catalog.GetMetadata(typeof(TestRecord));
            Assert.Same(a, b);
        }

        [ValueSet("test", "1", "s")]
        private class TestRecord
        {
            public string? Name { get; set; }
        }
    }
}
