namespace TransitNova.BusinessLayer.DTOs.WarehouseManager
{
    public sealed class WarehouseManagerListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
    }
}
