using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;
public sealed class UiPackageSpecificationDto
{
    public decimal Weight { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Length { get; set; }

    public static UiPackageSpecificationDto ToUiDto(PackageSpecificationDto source) =>
        new()
        {
            Weight = source.Weight,
            Width = source.Width,
            Height = source.Height,
            Length = source.Length
        };
}
