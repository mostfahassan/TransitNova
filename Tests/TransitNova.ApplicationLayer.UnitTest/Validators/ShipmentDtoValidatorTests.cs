using FluentAssertions;
using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Validators.ShipmentValidators;
using TransitNova.BusinessLayer.Validators.UserProfile.User;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Validators;

public sealed class ShipmentDtoValidatorTests
{
    [Fact]
    public async Task CreateReceiverValidator_CompleteReceiver_Should_BeValidAsync()
    {
        var result = await new CreateReceiverValidator().ValidateAsync(ValidReceiver());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("first-name")]
    [InlineData("last-name")]
    [InlineData("email")]
    [InlineData("phone")]
    [InlineData("address")]
    [InlineData("city")]
    public async Task CreateReceiverValidator_InvalidProfileField_Should_BeInvalidAsync(string field)
    {
        var receiver = ValidReceiver();
        switch (field)
        {
            case "first-name": receiver.FirstName = "1"; break;
            case "last-name": receiver.LastName = string.Empty; break;
            case "email": receiver.Email = "not-an-email"; break;
            case "phone": receiver.PhoneNumber = "000"; break;
            case "address": receiver.Address = string.Empty; break;
            case "city": receiver.CityId = 0; break;
        }

        var result = await new CreateReceiverValidator().ValidateAsync(receiver);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task PackageSpecificationValidator_PositiveDimensions_Should_BeValidAsync()
    {
        var result = await new PackageSpecificationValidator().ValidateAsync(ValidPackage());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("weight")]
    [InlineData("width")]
    [InlineData("height")]
    [InlineData("length")]
    public async Task PackageSpecificationValidator_NonPositiveDimension_Should_BeInvalidAsync(string dimension)
    {
        var package = ValidPackage();
        switch (dimension)
        {
            case "weight": package.Weight = 0; break;
            case "width": package.Width = -1; break;
            case "height": package.Height = 0; break;
            case "length": package.Length = -1; break;
        }

        var result = await new PackageSpecificationValidator().ValidateAsync(package);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateShipmentValidation_CompleteShipment_Should_BeValidAsync()
    {
        var result = await CreateValidator().ValidateAsync(ValidShipment());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("currency")]
    [InlineData("transportation")]
    [InlineData("delivery-type")]
    public async Task CreateShipmentValidation_InvalidEnum_Should_BeInvalidAsync(string field)
    {
        var shipment = ValidShipment();
        shipment = field switch
        {
            "currency" => shipment with { Currency = (Currency)999 },
            "transportation" => shipment with { TransportationMode = (TransportationMode)999 },
            _ => shipment with { ShipmentDeliveryType = (enShipmentType)999 }
        };

        var result = await CreateValidator().ValidateAsync(shipment);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("delivery")]
    [InlineData("pickup")]
    [InlineData("same-address")]
    [InlineData("bundle")]
    [InlineData("receiver")]
    [InlineData("package")]
    public async Task CreateShipmentValidation_MissingOrConflictingRequiredData_Should_BeInvalidAsync(string field)
    {
        var shipment = ValidShipment();
        shipment = field switch
        {
            "delivery" => shipment with { DeliveryAddress = string.Empty },
            "pickup" => shipment with { PickupAddress = string.Empty },
            "same-address" => shipment with { PickupAddress = shipment.DeliveryAddress.ToUpperInvariant() },
            "bundle" => shipment with { PackageBundleId = null },
            "receiver" => shipment with { Receiver = null! },
            _ => shipment with { PackageSpecification = null! }
        };

        var result = await CreateValidator().ValidateAsync(shipment);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("delivery")]
    [InlineData("pickup")]
    public async Task CreateShipmentValidation_AddressOverMaximumLength_Should_BeInvalidAsync(string field)
    {
        var shipment = ValidShipment();
        var overLimit = new string('a', 251);
        shipment = field == "delivery"
            ? shipment with { DeliveryAddress = overLimit }
            : shipment with { PickupAddress = overLimit };

        var result = await CreateValidator().ValidateAsync(shipment);

        result.IsValid.Should().BeFalse();
    }

    private static IValidator<CreateShipmentDto> CreateValidator() =>
        new CreateShipmentValidation(new CreateReceiverValidator());

    private static CreateReceiverDto ValidReceiver() => new()
    {
        FirstName = "Mona",
        LastName = "Ali",
        Email = "mona@example.com",
        PhoneNumber = "+201001234567",
        Address = "Cairo",
        CityId = 1,
        SenderId = Guid.NewGuid()
    };

    private static PackageSpecificationDto ValidPackage() => new()
    {
        Weight = 5,
        Width = 10,
        Height = 10,
        Length = 10
    };

    private static CreateShipmentDto ValidShipment() => new(
        ValidReceiver(),
        ValidPackage(),
        Currency.EGP,
        DateTime.UtcNow.AddDays(1),
        TransportationMode.Land,
        enShipmentType.Standard,
        "Alexandria Port",
        "Cairo Hub",
        Guid.NewGuid(),
        Guid.NewGuid(),
        PaymentMethod.CreditCard);
}
