using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Warehouse;
namespace TransitNovaUI.BusinessLayer.DTOs.Warehouse;

public sealed class UiCreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    public Address WarehouseAddress { get; init; } = null!;
    public decimal Capacity { get; set; }
    public decimal CurrentUsage { get; set; }
    public int OperatingHours { get; set; }
    public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];

    public static CreateWarehouseDto ToDto(UiCreateWarehouseDto source) =>
        new()
        {
            Name = source.Name,
            Type = source.Type,
            Address = source.WarehouseAddress,
            Capacity = source.Capacity,
            CurrentUsage = source.CurrentUsage,
            OperatingHours = source.OperatingHours,
            ZoneIds = [.. source.ZoneIds]
        };

}
