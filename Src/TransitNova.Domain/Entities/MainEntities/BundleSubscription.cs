
namespace TransitNova.Domain.Entities.MainEntities
{
    public class BundleSubscription 
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public bool IsActive { get; set; }
        public Guid SubscribedUserId { get;  set; }
        public virtual UserProfile SubscribedUser { get;} = null!;
        public Guid BundleId { get;  set; }
        public virtual Bundle Bundle { get;  set; } = null!;
        public DateTime SubscriptionDate { get;  set; }
        public DateTime? EndDate { get;  set; }
        public DateTime? CancelledAt { get;  set; }
    }
}
