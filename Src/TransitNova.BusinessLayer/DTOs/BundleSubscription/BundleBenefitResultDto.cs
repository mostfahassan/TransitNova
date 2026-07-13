namespace TransitNova.BusinessLayer.DTOs.BundleSubscription;

public sealed class BundleBenefitResultDto
{
    public Guid? BundleSubscriptionId { get; init; }
    public Guid? BundleId { get; init; }
    public string? BundleName { get; init; }
    public decimal OriginalShippingCost { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal FinalShippingCost { get; init; }
    public bool SubscriptionBenefitApplied { get; init; }
    public string SubscriptionBenefitMessage { get; init; } = string.Empty;

    public static BundleBenefitResultDto None(decimal originalShippingCost, string message = "No subscription benefit was applied.")
    {
        return new BundleBenefitResultDto
        {
            OriginalShippingCost = originalShippingCost,
            FinalShippingCost = originalShippingCost,
            SubscriptionBenefitMessage = message
        };
    }
}
