using TransitNova.BusinessLayer.DTOs.WarehouseManager;

namespace TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

public sealed class UiWarehouseManagerFilterDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public Guid? WarehouseId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public static WarehouseManagerFilterDto ToDto(UiWarehouseManagerFilterDto source) =>
        new()
        {
            FullName = source.FullName,
            Email = source.Email,
            WarehouseId = source.WarehouseId,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };
}
