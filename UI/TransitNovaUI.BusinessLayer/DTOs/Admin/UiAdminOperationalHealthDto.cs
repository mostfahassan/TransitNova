using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiAdminOperationalHealthDto
{
    public int AvailableCarriers { get; set; }
    public int BusyCarriers { get; set; }
    public int ActiveOperationManagers { get; set; }
    public decimal AverageCarrierRating { get; set; }
    public decimal DeliverySuccessRate { get; set; }
    public decimal CancelledShipmentRate { get; set; }

    public static UiAdminOperationalHealthDto ToUiDto(AdminOperationalHealthDto source) =>
        new()
        {
            AvailableCarriers = source.AvailableCarriers,
            BusyCarriers = source.BusyCarriers,
            ActiveOperationManagers = source.ActiveOperationManagers,
            AverageCarrierRating = source.AverageCarrierRating,
            DeliverySuccessRate = source.DeliverySuccessRate,
            CancelledShipmentRate = source.CancelledShipmentRate
        };
}
