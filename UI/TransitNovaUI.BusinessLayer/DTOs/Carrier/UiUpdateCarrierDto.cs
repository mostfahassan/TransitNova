using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiUpdateCarrierDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public int? CityId { get; set; }
    public UiAddressDto? Address { get; set; }
 

    public static UpdateCarrierDto ToDto(UiUpdateCarrierDto source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            CityId = source.CityId,
            Address = source.Address is null ? null : UiAddressDto.ToDto(source.Address),
           
        };

}
