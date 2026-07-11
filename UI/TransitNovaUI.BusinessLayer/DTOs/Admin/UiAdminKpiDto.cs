using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiAdminKpiDto
{
    public int TotalShipments { get; set; }
    public int ActiveShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int PendingShipments { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalTrips { get; set; }
    public int PlannedTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int TotalCarriers { get; set; }
    public int ActiveCarriers { get; set; }
    public int TotalOperationManagers { get; set; }
    public int ActiveTrips { get; set; }

    public static UiAdminKpiDto ToUiDto(AdminKpiDto source) =>
        new()
        {
            TotalShipments = source.TotalShipments,
            ActiveShipments = source.ActiveShipments,
            DeliveredShipments = source.DeliveredShipments,
            PendingShipments = source.PendingShipments,
            TotalUsers = source.TotalUsers,
            ActiveUsers = source.ActiveUsers,
            TotalTrips = source.TotalTrips,
            PlannedTrips = source.PlannedTrips,
            CompletedTrips = source.CompletedTrips,
            TotalCarriers = source.TotalCarriers,
            ActiveCarriers = source.ActiveCarriers,
            TotalOperationManagers = source.TotalOperationManagers,
            ActiveTrips = source.ActiveTrips,
           
        };
}
