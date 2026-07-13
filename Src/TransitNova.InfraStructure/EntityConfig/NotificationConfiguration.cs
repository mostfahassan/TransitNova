
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> notification)
        {
            notification.HasKey(x => x.Id).IsClustered();
            notification.HasIndex(x => x.UserId);
            notification.HasIndex(x => x.IsRead);
            notification.HasIndex(x => x.CreatedOnUtc);
        }
    }
}
