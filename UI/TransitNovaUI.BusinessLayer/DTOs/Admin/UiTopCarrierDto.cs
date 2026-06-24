using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiTopCarrierDto
{
    public Guid CarrierId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int DeliveredShipments { get; set; }
    public decimal Rating { get; set; }

    public static UiTopCarrierDto ToUiDto(TopCarrierDto source) =>
        new()
        {
            CarrierId = source.CarrierId,
            FullName = source.FullName,
            DeliveredShipments = source.DeliveredShipments,
            Rating = source.Rating
        };
}
