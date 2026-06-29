namespace TransitNova.BusinessLayer.DTOs.WarehouseManager
{
    public sealed class WarehouseManagerFilterDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public Guid? WarehouseId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
