using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNovaUI.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierTripDto
{
    public Guid Id { get; set; }
    public TripType TripType { get; set; }
    public TripStatus Status { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public UiAddressDto WarehouseAddress { get; set; } = new();
    public int ShipmentCount { get; set; }
    public IReadOnlyCollection<UiRetrieveShipmentDto> Shipments { get; set; } = [];

    public static UiCarrierTripDto ToUiDto(CarrierTripDto source) =>
        new()
        {
            Id = source.Id,
            TripType = source.TripType,
            Status = source.Status,
            PlannedDate = source.PlannedDate,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            WarehouseName = source.WarehouseName,
            WarehouseAddress = new UiAddressDto
            {
                MainAddress = source.WarehouseAddress.MainAddress,
                SecondaryAddress = source.WarehouseAddress.SecondaryAddress,
                Street = source.WarehouseAddress.Street
            },
            ShipmentCount = source.ShipmentCount,
            Shipments = source.Shipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList()
        };
}
