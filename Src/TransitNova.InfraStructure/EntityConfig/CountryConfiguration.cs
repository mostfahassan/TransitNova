using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> country)
        {
            country.HasKey(c => c.Id);

            country.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            country.HasMany(c => c.Governments)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
