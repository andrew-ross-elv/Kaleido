using System;
using System.Collections.Generic;
using Kaleido;
using Kaleido.Metadata;
using Xunit;

namespace Core.Tests
{
    public class ValueSetQueryCompilerTests
    {
        [Fact]
        public void Compile_DefaultsPageSizeTo50_WhenNoPageable()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var req = new QueryRequest(null, null);
            var compiler = new ValueSetQueryCompiler();
            var compiled = compiler.Compile(req, meta);
            Assert.Equal(50, compiled.Page.Size);
        }

        [Fact]
        public void Compile_UsesCursorDecode()
        {
            var pageable = new RuntimePageableMetadata(5, 10, true);
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), pageable);
            var req = new QueryRequest(null, new QueryBody(null, null, null, new QueryPage(2, CursorCodec.EncodeOffset(7))));
            var compiler = new ValueSetQueryCompiler();
            var compiled = compiler.Compile(req, meta);
            Assert.Equal(7, compiled.Page.Offset);
            Assert.Equal(2, compiled.Page.Size);
        }

        [Fact]
        public void Compile_UnsupportedFilter_Throws()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var req = new QueryRequest(null, new QueryBody(null, new CustomFilter(), null, null));
            var compiler = new ValueSetQueryCompiler();
            Assert.Throws<NotSupportedException>(() => compiler.Compile(req, meta));
        }

        [Fact]
        public void Compile_UnsupportedSearch_Throws()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var req = new QueryRequest(null, new QueryBody(new CustomSearch(), null, null, null));
            var compiler = new ValueSetQueryCompiler();
            Assert.Throws<NotSupportedException>(() => compiler.Compile(req, meta));
        }

        private class CustomFilter : IFilterExpression { }
        private class CustomSearch : ISearchExpression { }
    }
}
