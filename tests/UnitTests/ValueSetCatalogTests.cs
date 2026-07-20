using System;
using System.Collections.Generic;
using Kaleido;
using Kaleido.Metadata;
using Kaleido.Registry;
using Xunit;

namespace Core.Tests
{
    public class ValueSetCatalogTests
    {
        [Fact]
        public void GetAll_ReturnsDescriptors_ForAllRegistrations()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var reg1 = new ValueSetRegistration("a", typeof(object), meta);
            var reg2 = new ValueSetRegistration("b", typeof(string), meta);

            var registry = new TestRegistry(new[] { reg1, reg2 });
            var descriptors = new ValueSetDescriptorFactory();
            var dispatcher = new TestDispatcher();
            var catalog = new ValueSetCatalog(registry, dispatcher, descriptors);

            var all = catalog.GetAll();
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public void Get_ReturnsDescriptorOrNull()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var reg = new ValueSetRegistration("x", typeof(object), meta);
            var registry = new TestRegistry(new[] { reg });
            var descriptors = new ValueSetDescriptorFactory();
            var dispatcher = new TestDispatcher();
            var catalog = new ValueSetCatalog(registry, dispatcher, descriptors);

            Assert.NotNull(catalog.Get("x"));
            Assert.Null(catalog.Get("nope"));
        }

        [Fact]
        public async Task QueryAsync_DelegatesToDispatcher()
        {
            var meta = new RuntimeValueSetMetadata("n","v","s", Array.Empty<RuntimeFieldMetadata>(), Array.Empty<RuntimeAllowedQueryMetadata>(), null);
            var reg = new ValueSetRegistration("x", typeof(object), meta);
            var registry = new TestRegistry(new[] { reg });
            var descriptors = new ValueSetDescriptorFactory();
            var dispatcher = new TestDispatcher();
            var catalog = new ValueSetCatalog(registry, dispatcher, descriptors);

            var resp = await catalog.QueryAsync("x", new QueryRequest(null, null));
            Assert.Equal(0, resp.TotalCount);
        }

        private class TestRegistry : IValueSetRegistry
        {
            private readonly ValueSetRegistration[] _regs;
            public TestRegistry(ValueSetRegistration[] regs) => _regs = regs;
            public IReadOnlyCollection<ValueSetRegistration> Registrations => _regs;
            public ValueSetRegistration? Find(string valueSetKey) => Array.Find(_regs, r => r.Key == valueSetKey);
        }

        private class TestDispatcher : IValueSetDispatcher
        {
            public Task<ValueSetQueryResponse> QueryAsync(string valueSetKey, QueryRequest request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new ValueSetQueryResponse(new ValueSetDescriptor("n","v","s",Array.Empty<FieldDescriptor>(),Array.Empty<AllowedQueryDescriptor>(),null), 0, null, Array.Empty<object>()));
            }
        }
    }
}
