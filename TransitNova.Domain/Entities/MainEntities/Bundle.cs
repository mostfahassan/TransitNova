using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Bundle : AggregateRoot<Guid>
    {
        private readonly List<Shipment> _shipment = new ();
        public string BundleName { get; private set; } = string.Empty;
        public decimal BundlePrice { get; private set; }
        public string BundleDescription { get; private set; } = string.Empty;
        public decimal TotalWeight { get; private set; }
        public decimal TotalDistance { get; private set; }
        public int TotalShipments { get; private set; }
        public int BundleDurationMonths { get; private set; } = 1; 
        public virtual IReadOnlyCollection<Shipment> Shipments => _shipment;
        public virtual ICollection<BundleSubscription> Subscriptions { get; private set; } = new List<BundleSubscription>();

        private Bundle()
        {
            
        }

        private Bundle(string creatorId, string bundleName, decimal bundlePrice, string description, decimal totalWeight, decimal totalDistance, int totalShipments)
        {
            BundleName = bundleName;
            BundlePrice = bundlePrice;
            BundleDescription = description;
            TotalWeight = totalWeight;
            TotalDistance = totalDistance;
            TotalShipments = totalShipments;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = creatorId;
            CurrentState = true;
        }
        public static Bundle Create(string creatorId, string bundleName, decimal bundlePrice, string description, decimal totalWeight, decimal totalDistance, int totalShipments)
        {
            return new (creatorId,  bundleName,  bundlePrice,  description,  totalWeight,  totalDistance, totalShipments);
        }
        public void Update(string userId, decimal bundlePrice, decimal totalWeight, int totalShipments)
        {
            BundlePrice = bundlePrice;
            TotalWeight = totalWeight;
            TotalShipments = totalShipments;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;

            RaiseDomainEvent(new BundleUpdatedDomainEvent(Id, BundlePrice));
        }
        public void Subscribe(Guid userId)
        {
            if (Subscriptions.Any(s => s.SubscribedUserId == userId && s.IsActive))
                throw new DomainOperationException("User is already subscribed to this bundle.", "BUNDLE_ALREADY_SUBSCRIBED", "Bundle");
            var subscription = new BundleSubscription
            {
                SubscribedUserId = userId,
                BundleId = Id,
                SubscriptionDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(BundleDurationMonths), 
                IsActive = true
            };
            Subscriptions.Add(subscription);

            RaiseDomainEvent(new UserSubscribedToBundleDomainEvent(userId,Id));
        }

        public void Unsubscribe(Guid userId)
        {
            var subscription = Subscriptions.FirstOrDefault(s => s.SubscribedUserId == userId && s.IsActive)
                ?? throw new EntityNotFoundException("Subscription not found.", "BUNDLE_SUBSCRIPTION_NOT_FOUND", "Bundle");
            subscription.IsActive = false;
            subscription.CancelledAt = DateTime.UtcNow;
            RaiseDomainEvent(new UserUnSubscribedToBundleDomainEvent(userId, Id));
        }
    }
}