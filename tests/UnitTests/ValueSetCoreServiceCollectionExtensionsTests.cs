using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Kaleido;
using Kaleido.Validation;
using Kaleido.Registry;
using Kaleido.Metadata;

namespace Core.Tests
{
    public class ValueSetCoreServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddValueSetCore_Registers_Services()
        {
            var services = new ServiceCollection();
            services.AddKaleido();
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
