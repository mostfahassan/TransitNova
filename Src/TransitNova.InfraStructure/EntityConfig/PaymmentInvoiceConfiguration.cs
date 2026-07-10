
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    internal class PaymmentInvoiceConfiguration : IEntityTypeConfiguration<PaymentInvoice>
    {
        public void Configure(EntityTypeBuilder<PaymentInvoice> invoice)
        {
       
            invoice.HasKey(x => x.Id).IsClustered();

            invoice.Property(x => x.Status)
                   .HasConversion<string>() 
                   .HasMaxLength(50);

            invoice.Property(x => x.PaymentMethod)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            invoice.Property(x => x.Notes)
                   .HasMaxLength(1000); 

          
            invoice.Property(x => x.ShippingCost)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.Commission)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.Amount)
                   .HasColumnType("decimal(18, 2)");

    
            invoice.HasIndex(x => x.PaymentId);

            invoice.HasIndex(x => x.ShipmentId);

            invoice.HasIndex(x => x.CustomerId);

            invoice.HasIndex(x => x.CreatedAt);

            invoice.HasIndex(x => x.PaidAt);

            invoice.HasIndex(x => new { x.CustomerId, x.Status });
            invoice.HasIndex(x => new { x.ShipmentId, x.Status });

            invoice.HasOne(x => x.UserProfile)
                   .WithMany(x => x.PaymentInvoices)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


