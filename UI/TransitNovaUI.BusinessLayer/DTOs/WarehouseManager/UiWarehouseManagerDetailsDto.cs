using TransitNova.BusinessLayer.DTOs.WarehouseManager;

namespace TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

public sealed class UiWarehouseManagerDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiWarehouseManagerDetailsDto ToUiDto(WarehouseManagerDetailsDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            WarehouseId = source.WarehouseId,
            WarehouseName = source.WarehouseName,
            CreatedAt = source.CreatedAt
        };
}
