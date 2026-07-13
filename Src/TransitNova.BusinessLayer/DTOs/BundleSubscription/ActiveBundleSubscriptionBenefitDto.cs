namespace TransitNova.BusinessLayer.DTOs.BundleSubscription;

public sealed class ActiveBundleSubscriptionBenefitDto
{
    public Guid SubscriptionId { get; init; }
    public Guid BundleId { get; init; }
    public string BundleName { get; init; } = string.Empty;
    public DateTime SubscriptionDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int MaxShipmentsPerMonth { get; init; }
    public decimal MaxWeightPerShipment { get; init; }
    public decimal MaxDistancePerShipment { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal MinimumShipmentValueForDiscount { get; init; }
}
