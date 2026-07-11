using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNovaUI.BusinessLayer.Common.CommonData;

public sealed class UiCreateReceiverDto
{
    public string FirstName { get; set; } = string.Empty;

    public Guid SenderId { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public UiAddressDto Address { get; set; } = new();

    public int CityId { get; set; }

    public static CreateReceiverDto ToDto(UiCreateReceiverDto source) =>
        new()
        {
            FirstName = source.FirstName,
            SenderId = source.SenderId,
            LastName = source.LastName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = UiAddressDto.ToDto(source.Address),
            CityId = source.CityId
        };

}
