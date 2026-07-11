using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.BusinessLayer.DTOs.Warehouse
{
    public class UpdateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public WarehouseType Type { get; set; }
        public Address? Address { get; set; }
        public decimal Capacity { get; set; }
        public decimal CurrentUsage { get; set; }
        public int? OperatingHours { get; set; }
        public Guid? ManagerId { get; set; }
        public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];
    }
}
