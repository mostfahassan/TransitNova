using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.InfraStructure.EntityConfig;

internal static class AddressConfigurationExtensions
{
    internal static void OwnsAddress<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Address?>> navigation, string columnPrefix)
        where TEntity : class
    {
        builder.OwnsOne(navigation, address =>
        {
            address.Property(value => value.MainAddress)
                .HasColumnName($"{columnPrefix}_MainAddress")
                .HasMaxLength(Address.MainAddressMaxLength)
                .IsRequired();

            address.Property(value => value.SecondaryAddress)
                .HasColumnName($"{columnPrefix}_SecondaryAddress")
                .HasMaxLength(Address.SecondaryAddressMaxLength);

            address.Property(value => value.Street)
                .HasColumnName($"{columnPrefix}_Street")
                .HasMaxLength(Address.StreetMaxLength)
                .IsRequired();
        });

        builder.Navigation(navigation).IsRequired();
    }
}
