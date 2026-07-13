using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;

namespace TransitNova.BusinessLayer.Services.BundleService;

public sealed class BundleBenefitService(
    IBundleSubscriptionQueryRepository subscriptionRepository,
    ILogger<BundleBenefitService> logger) : IBundleBenefitService
{
    public async Task<BundleBenefitResultDto> CalculateShipmentBenefitAsync(Guid userProfileId, decimal originalShippingCost, PackageSpecificationDto packageSpecification, CancellationToken cancellationToken)
    {

        if (originalShippingCost <= 0)
            return BundleBenefitResultDto.None(originalShippingCost, "Shipment cost is not eligible for a subscription benefit.");

        var subscription = await subscriptionRepository.GetActiveSubscriptionForUserAsync(userProfileId, cancellationToken);
        if (subscription is null)
            return BundleBenefitResultDto.None(originalShippingCost);

        if (subscription.MaxWeightPerShipment > 0 && packageSpecification.Weight > subscription.MaxWeightPerShipment)
        {
            logger.LogInformation("Bundle {BundleId} was not applied for UserProfile {UserProfileId}: weight {Weight} exceeds limit {MaxWeight}.",
                subscription.BundleId,
                userProfileId,
                packageSpecification.Weight,
                subscription.MaxWeightPerShipment);

            return BundleBenefitResultDto.None(originalShippingCost, "Shipment weight is outside the active bundle limits.");
        }


        if (subscription.MinimumShipmentValueForDiscount > 0 && originalShippingCost < subscription.MinimumShipmentValueForDiscount)
        {
            return BundleBenefitResultDto.None(originalShippingCost, "Shipment value is below the active bundle discount threshold.");
        }


        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);
        var appliedThisMonth = await subscriptionRepository.GetMonthlyAppliedBenefitCountAsync(userProfileId,

            subscription.BundleId,
            monthStart,
            monthEnd,
            cancellationToken);

        if (subscription.MaxShipmentsPerMonth > 0 && appliedThisMonth >= subscription.MaxShipmentsPerMonth)
        {
            return BundleBenefitResultDto.None(originalShippingCost, "Monthly bundle shipment benefits have already been used.");
        }

        if (subscription.DiscountPercentage <= 0)
            return BundleBenefitResultDto.None(originalShippingCost, "The active bundle does not include a shipment discount.");

        var discountAmount = Math.Round(originalShippingCost * (subscription.DiscountPercentage / 100m), 2, MidpointRounding.AwayFromZero);
        var finalShippingCost = Math.Max(0m, originalShippingCost - discountAmount);

        return new BundleBenefitResultDto
        {
            BundleSubscriptionId = subscription.SubscriptionId,
            BundleId = subscription.BundleId,
            BundleName = subscription.BundleName,
            OriginalShippingCost = originalShippingCost,
            DiscountPercentage = subscription.DiscountPercentage,
            DiscountAmount = discountAmount,
            FinalShippingCost = finalShippingCost,
            SubscriptionBenefitApplied = discountAmount > 0,
            SubscriptionBenefitMessage = discountAmount > 0
                ? $"{subscription.BundleName} applied. You saved {discountAmount:N2} on this shipment."
                : "No subscription benefit was applied."
        };
    }
}
