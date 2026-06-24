using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

public sealed class UiRetrieveCarriersForOperationManagerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public CarrierStatus Status { get; set; }
    public int AssignedShipmentsCount { get; set; }
    public int ActiveTripsCount { get; set; }
    public List<string> ServedCities { get; set; } = [];
    public decimal Rating { get; set; }

    public static UiRetrieveCarriersForOperationManagerDto ToUiDto(
        RetrieveCarriersForOperationManagerDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            PhoneNumber = source.PhoneNumber,
            Status = source.Status,
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            ServedCities = [.. source.ServedCities],
            Rating = source.Rating
        };
}
