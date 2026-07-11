using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Common.CommonData;

public sealed class AddressDto
{
    public string MainAddress { get; set; } = string.Empty;
    public string? SecondaryAddress { get; set; }
    public string Street { get; set; } = string.Empty;

    public Address ToDomain()
        => Address.Create(MainAddress, SecondaryAddress, Street);

    public static AddressDto FromDomain(Address address)
        => new()
        {
            MainAddress = address.MainAddress,
            SecondaryAddress = address.SecondaryAddress,
            Street = address.Street
        };

    public string ToNormalizedString()
        => string.Join("|", new[] { MainAddress, SecondaryAddress, Street }
            .Select(value => value?.Trim().ToUpperInvariant() ?? string.Empty));

    public override string ToString()
        => string.Join(", ", new[] { MainAddress, SecondaryAddress, Street }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
}
