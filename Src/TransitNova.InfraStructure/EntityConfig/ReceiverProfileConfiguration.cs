using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public partial class ReceiverProfileConfiguration : IEntityTypeConfiguration<ReceiverProfile>
    {
        public void Configure(EntityTypeBuilder<ReceiverProfile> user)
        {
            user.HasKey(u => u.Id).HasName("ReceiverProfileKey"); 
                
            user.HasMany(u => u.ReceivedShipments)
            .WithOne(s => s.Receiver)
            .HasForeignKey(s => s.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(r => r.Sender)
                .WithMany(s => s.Receivers);

            user.HasOne(r => r.Sender)
                .WithMany(s => s.Receivers)
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            user.HasIndex(u => u.SenderId)
                .HasDatabaseName("IX_ReceiverProfiles_SenderId");
            
            user.HasIndex(u => u.CityId);
            user.HasIndex(u => u.Email);
 
        }
   
    }
}
