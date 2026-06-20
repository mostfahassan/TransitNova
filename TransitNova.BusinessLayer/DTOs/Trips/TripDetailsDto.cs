using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.DTOs.Trips
{
    public sealed record TripDetailsDto
    {
        public Guid Id { get; init; }
        public Guid CarrierId { get; init; }
        public string CarrierName { get; init; } = string.Empty;
        public string CarrierPhoneNumber { get; init; } = string.Empty;
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public string WarehouseAddress { get; init; } = string.Empty;
        public TripType TripType { get; init; }
        public TripStatus Status { get; init; }
        public DateTime PlannedDate { get; init; }
        public DateTime? StartTime { get; init; }
        public DateTime? EndTime { get; init; }
        public int TotalShipments { get; init; }
        public IReadOnlyCollection<RetrieveShipmentDto> Shipments { get; init; } = [];
    }
}
