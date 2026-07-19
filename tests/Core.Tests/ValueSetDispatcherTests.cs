using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Kaleido.Abstractions;
using Kaleido.Core;
using Xunit;

namespace Core.Tests
{
    public class ValueSetDispatcherTests
    {
        [Fact]
        public async Task QueryAsync_InvokesEngine_And_ReturnsResponse()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IValueSetDescriptorFactory, ValueSetDescriptorFactory>();
            services.AddSingleton<IValueSetRegistry>(new TestRegistry());
            services.AddScoped<IValueSetQueryEngine<SampleRecord>, SampleEngine>();

            var provider = services.BuildServiceProvider();
            var registry = provider.GetRequiredService<IValueSetRegistry>();
            var descriptors = provider.GetRequiredService<IValueSetDescriptorFactory>();
            var dispatcher = new ValueSetDispatcher(provider, registry, descriptors);

            var response = await dispatcher.QueryAsync("sample", new QueryRequest(null, null));

            Assert.NotNull(response);
            Assert.Equal(1, response.TotalCount);
            Assert.Single(response.Items);
            Assert.Equal("hello", ((SampleRecord)response.Items[0]).Text);
        }

        private sealed class TestRegistry : IValueSetRegistry
        {
            private readonly RuntimeValueSetMetadata _meta = new("sample","1","s",Array.Empty<RuntimeFieldMetadata>(),Array.Empty<RuntimeAllowedQueryMetadata>(),null);
            public IReadOnlyCollection<ValueSetRegistration> Registrations => new[] { new ValueSetRegistration("sample", typeof(SampleRecord), _meta) };
            public ValueSetRegistration? Find(string valueSetKey) => valueSetKey == "sample" ? new ValueSetRegistration("sample", typeof(SampleRecord), _meta) : null;
        }

        private class SampleEngine : IValueSetQueryEngine<SampleRecord>
        {
            public Task<QueryResult<SampleRecord>> ExecuteAsync(QueryRequest request, CancellationToken cancellationToken = default)
            {
                var items = new List<SampleRecord> { new SampleRecord { Text = "hello" } };
                var result = new QueryResult<SampleRecord>(items, 1, null, new RuntimeValueSetMetadata("sample","1","s",Array.Empty<RuntimeFieldMetadata>(),Array.Empty<RuntimeAllowedQueryMetadata>(),null));
                return Task.FromResult(result);
            }
        }

        private class SampleRecord { public string? Text { get; set; } }
    }
}
