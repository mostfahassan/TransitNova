using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig;
public sealed class SystemActivityLogConfiguration : IEntityTypeConfiguration<SystemActivityLog>
{
    public void Configure(EntityTypeBuilder<SystemActivityLog> Log)
    {
        Log.ToTable("SystemActivityLogs");

        Log.HasKey(x => x.Id);

        Log.Property(x => x.EntityType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        Log.Property(x => x.Action)
             .HasConversion<string>()
             .IsRequired()
             .HasMaxLength(15);

        Log.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        Log.Property(x => x.PerformedByName)
            .IsRequired()
            .HasMaxLength(150);

        Log.Property(x => x.OccurredAt)
            .IsRequired();

        Log.HasIndex(x => x.OccurredAt);
        Log.HasIndex(x => new {x.PerformedByName, x.OccurredAt});
        Log.HasIndex(x => x.EntityType);
        Log.HasIndex(x => x.PerformedByUserId);
    }
}