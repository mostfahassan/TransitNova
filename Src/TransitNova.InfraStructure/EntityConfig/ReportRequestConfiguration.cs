using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> reportRequest)
        {
            reportRequest.HasKey(r => r.Id);

            reportRequest.Property(r => r.ReportKey)
                .HasMaxLength(128)
                .IsRequired();

            reportRequest.Property(r => r.PayloadJson)
                .IsRequired();

            reportRequest.Property(r => r.FilePath)
                .HasMaxLength(1024);

            reportRequest.Property(r => r.ErrorMessage)
                .HasMaxLength(4000);
        }
    }
}
