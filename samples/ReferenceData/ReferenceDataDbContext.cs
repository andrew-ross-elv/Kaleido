using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReferenceData.ValueSets;
using System.Reflection.Emit;

namespace ReferenceData
{
    public sealed class KaleidoTestDbContext : DbContext
    {
        public KaleidoTestDbContext(DbContextOptions<KaleidoTestDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientRecord> Clients => Set<ClientRecord>();
        //public DbSet<ProductRecord> Products => Set<ProductRecord>();
        //public DbSet<ClientProductRecord> ClientProducts => Set<ClientProductRecord>();
        //public DbSet<SyntheticFactRecord> SyntheticFacts => Set<SyntheticFactRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                v => v.ToString("yyyy-MM-dd"),
                v => DateOnly.Parse(v));

            var nullableDateOnlyConverter = new ValueConverter<DateOnly?, string?>(
                v => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : null,
                v => v == null ? null : DateOnly.Parse(v));

            modelBuilder.Entity<ClientRecord>(entity =>
            {
                entity.ToTable("Clients");
                entity.HasKey(x => x.ClientId);
            });

            //modelBuilder.Entity<ProductRecord>(entity =>
            //{
            //    entity.ToTable("Products");
            //    entity.HasKey(x => x.ProductId);
            //});

            //modelBuilder.Entity<ClientProductRecord>(entity =>
            //{
            //    entity.ToTable("ClientProducts");
            //    entity.HasKey(x => x.ClientProductId);
            //    entity.Property(x => x.EffectiveStart).HasConversion(dateOnlyConverter);
            //    entity.Property(x => x.EffectiveEnd).HasConversion(nullableDateOnlyConverter);
            //});

            //modelBuilder.Entity<SyntheticFactRecord>(entity =>
            //{
            //    entity.ToTable("SyntheticFacts");
            //    entity.HasKey(x => x.FactId);
            //    entity.Property(x => x.ExternalId).HasConversion(v => v.ToString(), v => Guid.Parse(v));
            //    entity.Property(x => x.EffectiveDate).HasConversion(dateOnlyConverter);
            //    entity.Property(x => x.ExpirationDate).HasConversion(nullableDateOnlyConverter);
            //    entity.Property(x => x.Status).HasConversion<string>();
            //});
        }
    }
}
