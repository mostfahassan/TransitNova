using TransitNova.BusinessLayer.DTOs.Bundle;
namespace TransitNovaUI.BusinessLayer.DTOs.Bundle;

public sealed class UiCreateBundleDto
{
    public string BundleName { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal BundlePrice { get; set; }
    public string BundleDescription { get; set; } = string.Empty;
    public decimal TotalDistance { get; set; }
    public int TotalShipments { get; set; }

    public static CreateBundleDto ToDto(UiCreateBundleDto source) =>
        new()
        {
            BundleName = source.BundleName,
            TotalWeight = source.TotalWeight,
            BundlePrice = source.BundlePrice,
            BundleDescription = source.BundleDescription,
            TotalDistance = source.TotalDistance,
            TotalShipments = source.TotalShipments
        };

}
