using Microsoft.Extensions.DependencyInjection;
using Kaleido.Abstractions;
using Kaleido.Core;
using Xunit;

namespace Core.Tests
{
    public class ValueSetCoreServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddValueSetCore_Registers_Services()
        {
            var services = new ServiceCollection();
            services.AddValueSetCore();
            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IValueSetMetadataCatalog>());
            Assert.NotNull(provider.GetService<IValueSetDescriptorFactory>());
            Assert.NotNull(provider.GetService<IValueSetQueryValidator>());
            Assert.NotNull(provider.GetService<IValueSetQueryCompiler>());
            Assert.NotNull(provider.GetService<IValueSetRegistry>());
            // scoped services should be resolvable
            using (var scope = provider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IValueSetDispatcher>());
                Assert.NotNull(scope.ServiceProvider.GetService<IValueSetCatalog>());
            }
        }
    }
}
