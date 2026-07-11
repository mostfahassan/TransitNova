using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNovaUI.BusinessLayer.Common.CommonData;

public sealed class UiAddressDto
{
    public string MainAddress { get; set; } = string.Empty;
    public string? SecondaryAddress { get; set; }
    public string Street { get; set; } = string.Empty;

    public static AddressDto ToDto(UiAddressDto source)
        => new()
        {
            MainAddress = source.MainAddress,
            SecondaryAddress = source.SecondaryAddress,
            Street = source.Street
        };

    public static UiAddressDto ToUiDto(AddressDto source)
        => new()
        {
            MainAddress = source.MainAddress,
            SecondaryAddress = source.SecondaryAddress,
            Street = source.Street
        };

    public override string ToString()
        => string.Join(", ", new[] { MainAddress, SecondaryAddress, Street }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
}
