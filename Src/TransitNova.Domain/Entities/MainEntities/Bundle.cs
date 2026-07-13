using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Bundle;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Bundle : AggregateRoot<Guid>
    {
        public string BundleName { get; private set; } = string.Empty;
        public string BundleDescription { get; private set; } = string.Empty;
        public decimal BundlePrice { get; private set; }
        public BundleTier Tier { get; private set; }
        public int BundleDurationMonths { get; private set; } = 1;
        public int MaxShipmentsPerMonth { get; private set; }
        public decimal MaxWeightPerShipment { get; private set; }
        public decimal MaxDistancePerShipment { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal MinimumShipmentValueForDiscount { get; private set; }

        private readonly List<BundleSubscription> _subscriptions = new();
        public virtual IReadOnlyCollection<BundleSubscription> Subscriptions => _subscriptions.AsReadOnly();


        private Bundle() { }

        private Bundle(string creatorId, string bundleName, string description, decimal bundlePrice,
            BundleTier tier, int durationMonths, int maxShipments, decimal maxWeight,
            decimal maxDistance, decimal discountPercentage, decimal minShipmentValue)
        {
            Id = Guid.CreateVersion7();
            BundleName = bundleName;
            BundleDescription = description;
            BundlePrice = bundlePrice;
            Tier = tier;
            BundleDurationMonths = durationMonths;
            MaxShipmentsPerMonth = maxShipments;
            MaxWeightPerShipment = maxWeight;
            MaxDistancePerShipment = maxDistance;
            DiscountPercentage = discountPercentage;
            MinimumShipmentValueForDiscount = minShipmentValue;
            CreatedBy = creatorId;
        }

      
        public static Bundle Create(string creatorId, string bundleName, string description, decimal bundlePrice,
            BundleTier tier, int durationMonths, int maxShipments, decimal maxWeight,
            decimal maxDistance, decimal discountPercentage, decimal minShipmentValue)
        {
            return new Bundle(creatorId, bundleName, description, bundlePrice, tier, durationMonths,
                              maxShipments, maxWeight, maxDistance, discountPercentage, minShipmentValue);
        }

    

        public void Update(string userId, decimal bundlePrice, string description, decimal discountPercentage, decimal minShipmentValue)
        {
            BundlePrice = bundlePrice;
            BundleDescription = description;
            DiscountPercentage = discountPercentage;
            MinimumShipmentValueForDiscount = minShipmentValue;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
            RaiseDomainEvent(new BundleUpdatedDomainEvent(Id, BundlePrice));
        }

        public void Subscribe(Guid userProfileId)
        {
            if (Subscriptions.Any(s => s.SubscribedUserId == userProfileId && s.IsActive))
                throw new DomainOperationException("User is already subscribed to this bundle.", "BUNDLE_ALREADY_SUBSCRIBED", "Bundle");

            var subscription = new BundleSubscription
            {
                SubscribedUserId = userProfileId,
                BundleId = Id,
                SubscriptionDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(BundleDurationMonths),
                IsActive = true
            };
            _subscriptions.Add(subscription);
            RaiseDomainEvent(new UserSubscribedToBundleDomainEvent(userProfileId, Id));
        }

        public void Unsubscribe(Guid userProfileId)
        {
            var subscription = Subscriptions.FirstOrDefault(s => s.SubscribedUserId == userProfileId && s.IsActive)
                ?? throw new EntityNotFoundException("Active subscription not found for this user.", "BUNDLE_SUBSCRIPTION_NOT_FOUND", "Bundle");

            subscription.IsActive = false;
            subscription.CancelledAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserUnsubscribedFromBundleDomainEvent(userProfileId, Id));
        }

        //=== Business Logic Methods 
        public decimal CalculateDiscountedPrice(decimal originalShipmentPrice)
        {
            if (DiscountPercentage > 0 && originalShipmentPrice >= MinimumShipmentValueForDiscount)
            {
                var discountAmount = originalShipmentPrice * (DiscountPercentage / 100);
                return originalShipmentPrice - discountAmount;
            }

            return originalShipmentPrice; 
        }
        public bool IsShipmentWithinBundleLimits(decimal weight, decimal distance)
        {
            return weight <= MaxWeightPerShipment && distance <= MaxDistancePerShipment;
        }
    }
}