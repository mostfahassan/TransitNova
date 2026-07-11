using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.Domain.Enums.Bundle;
namespace TransitNovaUI.BusinessLayer.DTOs.Bundle;

public sealed class UiUpdateBundleDto
{
    public Guid BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public string BundleDescription { get; set; } = string.Empty;
    public decimal BundlePrice { get; set; }
    public BundleTier Tier { get; set; }
    public int BundleDurationMonths { get; set; }

    public int MaxShipmentsPerMonth { get; set; }
    public decimal MaxWeightPerShipment { get; set; }
    public decimal MaxDistancePerShipment { get; set; }

    public decimal DiscountPercentage { get; set; }
    public decimal MinimumShipmentValueForDiscount { get; set; }

    public static UpdateBundleDto ToDto(UiUpdateBundleDto source) =>
    new()
    {
        BundleName = source.BundleName,
        BundleDescription = source.BundleDescription,
        BundlePrice = source.BundlePrice,
        Tier = source.Tier,
        BundleDurationMonths = source.BundleDurationMonths,
        MaxShipmentsPerMonth = source.MaxShipmentsPerMonth,
        MaxWeightPerShipment = source.MaxWeightPerShipment,
        MaxDistancePerShipment = source.MaxDistancePerShipment,
        DiscountPercentage = source.DiscountPercentage,
        MinimumShipmentValueForDiscount = source.MinimumShipmentValueForDiscount
    };
}
