using TransitNova.Domain.DomainExceptions;

namespace TransitNova.Domain.Entities.Common;

public sealed record Address
{
    public const int MainAddressMaxLength = 250;
    public const int SecondaryAddressMaxLength = 250;
    public const int StreetMaxLength = 150;

    public string MainAddress { get; private set; } = string.Empty;
    public string? SecondaryAddress { get; private set; }
    public string Street { get; private set; } = string.Empty;

    private Address()
    {
    }

    private Address(string mainAddress, string? secondaryAddress, string street)
    {
        MainAddress = mainAddress;
        SecondaryAddress = secondaryAddress;
        Street = street;
    }

    public static Address Create(string mainAddress, string? secondaryAddress, string street)
    {
        var normalizedMainAddress = NormalizeRequired(mainAddress, nameof(MainAddress), MainAddressMaxLength);
        var normalizedSecondaryAddress = NormalizeOptional(secondaryAddress, nameof(SecondaryAddress), SecondaryAddressMaxLength);
        var normalizedStreet = NormalizeRequired(street, nameof(Street), StreetMaxLength);

        return new Address(normalizedMainAddress, normalizedSecondaryAddress, normalizedStreet);
    }

    public static Address FromLegacy(string address)
        => Create(address, null, "Unknown");

    public override string ToString()
        => string.Join(", ", new[] { MainAddress, SecondaryAddress, Street }
            .Where(value => !string.IsNullOrWhiteSpace(value)));

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainOperationException($"{fieldName} is required.", "INVALID_ADDRESS");

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new DomainOperationException($"{fieldName} cannot exceed {maxLength} characters.", "INVALID_ADDRESS");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new DomainOperationException($"{fieldName} cannot exceed {maxLength} characters.", "INVALID_ADDRESS");

        return normalized;
    }
}
