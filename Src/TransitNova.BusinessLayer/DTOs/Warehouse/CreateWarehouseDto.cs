using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.BusinessLayer.DTOs.Warehouse
{
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public WarehouseType Type { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public decimal CurrentUsage { get; set; }
        public int OperatingHours { get; set; }
        public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];
        public Guid ManagerId { get; set; }
    }
}
