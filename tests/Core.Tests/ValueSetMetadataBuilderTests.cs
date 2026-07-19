using System;
using Kaleido.Abstractions;
using Kaleido.Abstractions.Attributes;
using Kaleido.Core;
using Xunit;

namespace Core.Tests
{
    public class ValueSetMetadataBuilderTests
    {
        [Fact]
        public void Build_WithoutAttribute_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => ValueSetMetadataBuilder.Build(typeof(NoAttributeRecord)));
        }

        [Fact]
        public void Build_WithAttributes_BuildsMetadata()
        {
            var meta = ValueSetMetadataBuilder.Build(typeof(RecordWithAttributes));
            Assert.Equal("myset", meta.Name);
            Assert.Single(meta.Fields);
            var f = meta.Fields[0];
            Assert.True(f.IsFilterable);
            Assert.True(f.IsSearchable);
            Assert.True(f.IsSortable);
        }

        private class NoAttributeRecord { public string? X { get; set; } }

        [ValueSet("myset", "1", "s")]
        [Pageable(10, 100, true)]
        private class RecordWithAttributes
        {
            [Filterable(FilterOperator.Eq)]
            [Searchable(1, MatchMode.Contains)]
            [Sortable(SortDirection.Asc)]
            public string? Name { get; set; }
        }
    }
}
