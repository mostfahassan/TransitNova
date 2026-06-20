using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class CarrierCompanyConfiguration : IEntityTypeConfiguration<CarrierCompany>
    {
        public void Configure(EntityTypeBuilder<CarrierCompany> carrierCompany)
        {


            carrierCompany.HasKey(cc => cc.Id);

            carrierCompany.Property(cc => cc.Name)
                .IsRequired()
                .HasMaxLength(100);

            carrierCompany.Property(cc => cc.Code)
                .IsRequired()
                .HasMaxLength(20);

            carrierCompany.Property(cc => cc.Phone)
                .HasMaxLength(15);

            carrierCompany.Property(cc => cc.Email)
                .HasMaxLength(50);

            carrierCompany.Property(cc => cc.Address)
                .HasMaxLength(200);

            carrierCompany.HasOne(cc => cc.Zone)
                .WithMany()
                .HasForeignKey(cc => cc.ZoneId)
                .OnDelete(DeleteBehavior.SetNull);

            carrierCompany.HasIndex(cc => cc.Code).IsUnique();


            carrierCompany.HasMany(cc => cc.Carriers)
            .WithOne(c => c.Company)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }


















}
