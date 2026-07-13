using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNovaUI.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Warehouse;
namespace TransitNovaUI.BusinessLayer.DTOs.Warehouse;

public sealed class UiWarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    public UiAddressDto Address { get; init; } = new();
    public decimal Capacity { get; set; }
    public decimal CurrentUsage { get; set; }
    public int? OperatingHours { get; set; }
    public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];
    public IReadOnlyCollection<string> ZoneNames { get; set; } = [];
    public int CarrierCount { get; set; }
    public int ActiveTripsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static UiWarehouseDto ToUiDto(WarehouseDto source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            Type = source.Type,
            Address = new UiAddressDto
            {
                MainAddress = source.Address.MainAddress,
                SecondaryAddress = source.Address.SecondaryAddress,
                Street = source.Address.Street
            },
            Capacity = source.Capacity,
            CurrentUsage = source.CurrentUsage,
            OperatingHours = source.OperatingHours,
            ZoneIds = [.. source.ZoneIds],
            ZoneNames = [.. source.ZoneNames],
            CarrierCount = source.CarrierCount,
            ActiveTripsCount = source.ActiveTripsCount,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
}
