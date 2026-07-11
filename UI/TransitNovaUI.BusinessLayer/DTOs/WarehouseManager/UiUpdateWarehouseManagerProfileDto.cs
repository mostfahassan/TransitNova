using TransitNova.BusinessLayer.DTOs.UserProfile;

using TransitNovaUI.BusinessLayer.Common.CommonData;

namespace TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

public sealed class UiUpdateWarehouseManagerProfileDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? CityId { get; set; }
    public UiAddressDto? Address { get; set; }
    public Guid? WarehouseId { get; set; }

    public static UpdateWarehouseManagerProfile ToDto(UiUpdateWarehouseManagerProfileDto source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            CityId = source.CityId,
            Address = source.Address is null ? null : UiAddressDto.ToDto(source.Address),
            WarehouseId = source.WarehouseId
        };
}
