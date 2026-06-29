using TransitNova.Domain.Enums.Warehouse;
namespace TransitNova.BusinessLayer.DTOs.Warehouse
{
    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public WarehouseType Type { get; set; }
        public string WarehouseManagerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public decimal CurrentUsage { get; set; }
        public int? OperatingHours { get; set; }
        public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];
        public IReadOnlyCollection<string> ZoneNames { get; set; } = [];
        public int CarrierCount { get; set; }
        public int ActiveTripsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
