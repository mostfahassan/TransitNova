using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNovaUI.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNovaUI.BusinessLayer.DTOs.Trips;

public sealed class UiTripDetailsDto
{
    public Guid Id { get; init; }
    public Guid CarrierId { get; init; }
    public string CarrierName { get; init; } = string.Empty;
    public string CarrierPhoneNumber { get; init; } = string.Empty;
    public Guid WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public UiAddressDto WarehouseAddress { get; init; } = new();
    public TripType TripType { get; init; }
    public TripStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime PlannedDate { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TotalShipments { get; init; }
    public IReadOnlyCollection<UiRetrieveShipmentDto> Shipments { get; init; } = [];

    public static UiTripDetailsDto ToUiDto(TripDetailsDto source) =>
        new()
        {
            Id = source.Id,
            CarrierId = source.CarrierId,
            CarrierName = source.CarrierName,
            CarrierPhoneNumber = source.CarrierPhoneNumber,
            WarehouseId = source.WarehouseId,
            WarehouseName = source.WarehouseName,
            WarehouseAddress = new UiAddressDto
            {
                MainAddress = source.WarehouseAddress.MainAddress,
                SecondaryAddress = source.WarehouseAddress.SecondaryAddress,
                Street = source.WarehouseAddress.Street
            },
            TripType = source.TripType,
            Status = source.Status,
            CreatedAt = source.CreatedAt,
            CreatedBy = source.CreatedBy,
            PlannedDate = source.PlannedDate,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            TotalShipments = source.TotalShipments,
            Shipments = source.Shipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList()
        };
}
