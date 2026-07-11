using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class UserBundleSubscriptionConfiguration : IEntityTypeConfiguration<BundleSubscription>
    {
        public void Configure(EntityTypeBuilder<BundleSubscription> userBundleSubscriptions)
        {

            userBundleSubscriptions.HasKey(ubs => ubs.Id);

            userBundleSubscriptions.HasOne(ubs => ubs.SubscribedUser)
                .WithMany(up => up.Subscriptions)
                .HasForeignKey(ubs => ubs.SubscribedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            userBundleSubscriptions.HasOne(ubs => ubs.Bundle)
                .WithMany(b => b.Subscriptions)
                .HasForeignKey(ubs => ubs.BundleId)
                .OnDelete(DeleteBehavior.Restrict);

            userBundleSubscriptions.HasIndex(x => new { x.SubscribedUserId, x.BundleId })
                                   .IsUnique()
                                   .HasFilter("[IsActive] = 1"); 

            userBundleSubscriptions.HasIndex(x => x.EndDate);

            userBundleSubscriptions.Property(x => x.SubscriptionDate)
                                   .IsRequired();

            userBundleSubscriptions.Property(x => x.EndDate)
                                   .IsRequired();
        }
    }
}
