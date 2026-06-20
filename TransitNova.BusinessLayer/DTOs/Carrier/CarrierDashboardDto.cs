using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class CarrierDashboardDto
    {
        public CarrierProfileDto Profile { get; set; } = new();
        public int AssignedShipmentsCount { get; set; }
        public int DeliveredShipmentsCount { get; set; }
        public int PendingShipmentsCount { get; set; }
        public int ActiveTripsCount { get; set; }
        public decimal RevenueSummary { get; set; }
        public IReadOnlyCollection<CarrierStatusStatDto> ShipmentStatistics { get; set; } = [];
        public IReadOnlyCollection<RetrieveShipmentDto> RecentShipments { get; set; } = [];
        public IReadOnlyCollection<CarrierTripDto> ActiveTrips { get; set; } = [];
        public IReadOnlyCollection<CarrierActivityDto> RecentActivity { get; set; } = [];
    }

    public class CarrierStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }

    public sealed record ChangeCarrierStatus(Guid Id ,CarrierStatus Status);
    public class CarrierActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ShipmentStatuses? Status { get; set; }
        public DateTime OccurredAt { get; set; }
    }

    public class CarrierTripDto
    {
        public Guid Id { get; set; }
        public TripType TripType { get; set; }
        public TripStatus Status { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseAddress { get; set; } = string.Empty;
        public int ShipmentCount { get; set; }
        public IReadOnlyCollection<RetrieveShipmentDto> Shipments { get; set; } = [];
    }

    public class CarrierShipmentFilterDto
    {
        public ShipmentStatuses? Status { get; set; }
        public TransportationMode? Mode { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class CarrierShipmentListDto
    {
        public PagedResult<RetrieveShipmentDto> Shipments { get; set; } =
            PagedResult<RetrieveShipmentDto>.From([], 0, 1, 12);
        public IReadOnlyCollection<CarrierStatusStatDto> Statistics { get; set; } = [];
    }

    public class UpdateCarrierVehicleDto
    {
        public VehicleType? VehicleType { get; set; }
        public string? PlateNumber { get; set; }
        public decimal? CapacityWeight { get; set; }
        public decimal? CapacityVolume { get; set; }
        public bool? IsRefrigerated { get; set; }
    }

    public class CarrierVehicleDto
    {
        public Guid Id { get; set; }
        public VehicleType VehicleType { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
        public bool IsRefrigerated { get; set; }
        public bool IsActive { get; set; }
    }
}
