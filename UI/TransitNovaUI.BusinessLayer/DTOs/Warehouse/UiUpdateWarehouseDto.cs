using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.Domain.Enums.Warehouse;
using TransitNovaUI.BusinessLayer.Common.CommonData;

namespace TransitNovaUI.BusinessLayer.DTOs.Warehouse;

public sealed class UiUpdateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    public UiAddressDto WarehouseAddress { get; init; } = new();
    public decimal Capacity { get; set; }
    public decimal CurrentUsage { get; set; }
    public int? OperatingHours { get; set; }
    public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];

    public static UpdateWarehouseDto ToDto(UiUpdateWarehouseDto source) =>
        new()
        {
            Name = source.Name,
            Type = source.Type,
            Address = UiAddressDto.ToDto(source.WarehouseAddress),
            Capacity = source.Capacity,
            CurrentUsage = source.CurrentUsage,
            OperatingHours = source.OperatingHours,
            ZoneIds = [.. source.ZoneIds]
        };
}
