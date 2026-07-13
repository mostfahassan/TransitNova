using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig;

public sealed class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> admin)
    {
        admin.HasKey(profile => profile.Id).IsClustered();
        admin.OwnsAddress(profile => profile.Address, "Address");

        admin.HasOne(profile => profile.City)
            .WithMany()
            .HasForeignKey(profile => profile.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        admin.Property(admin => admin.UserType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(6);

        admin.HasIndex(profile => profile.AppUserId).IsUnique();
        admin.HasIndex(profile => profile.CityId);
        admin.HasIndex(profile => profile.Email);
    }
}
