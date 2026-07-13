
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.InfraStructure.OutBox;
namespace TransitNova.InfraStructure.EntityConfig
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(x => x.Id).IsClustered();
            builder.HasIndex(x => x.OccuredAt);
            builder.HasIndex(x => x.ProcessedOn);
        }
    }
}
