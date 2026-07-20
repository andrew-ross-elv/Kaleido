using System;
using System.Linq;
using Kaleido;
using Kaleido.Metadata;
using Xunit;

namespace Core.Tests
{
    public class ValueSetRegistryTests
    {
        [Fact]
        public void Registrations_Grouping_LastWins_And_Find_IsCaseInsensitive()
        {
            var meta = new RuntimeValueSetMetadata("a","1","s",Array.Empty<RuntimeFieldMetadata>(),Array.Empty<RuntimeAllowedQueryMetadata>(),null);
            var list = new[]
            {
                new ValueSetRegistration("key", typeof(object), meta),
                new ValueSetRegistration("KEY", typeof(string), meta)
            };

            var reg = new ValueSetRegistry(list);
            var regs = reg.Registrations.ToArray();
            Assert.Single(regs);
            Assert.Equal(typeof(string), regs[0].RecordType);
            Assert.NotNull(reg.Find("key"));
            Assert.NotNull(reg.Find("KEY"));
            Assert.Equal(typeof(string), reg.Find("kEy")!.RecordType);
        }
    }
}
