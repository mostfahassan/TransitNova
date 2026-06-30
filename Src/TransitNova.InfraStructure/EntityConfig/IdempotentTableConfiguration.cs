using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public sealed class IdempotentTableConfiguration : IEntityTypeConfiguration<IdempotentTable>
    {
        public void Configure(EntityTypeBuilder<IdempotentTable> idempotent)
        {
            idempotent.HasKey(i => i.RequestId).IsClustered();

            idempotent.Property(p => p.InstanceName)
                .IsRequired()
                .HasMaxLength(30);

            idempotent.HasIndex(i => i.RequestId).IsUnique();
            idempotent.HasIndex(p => p.InstanceName);
            idempotent.HasIndex(p => p.CreatedAt);
            idempotent.HasIndex(p => new { p.InstanceName, p.CreatedAt });

        }
    }
}
