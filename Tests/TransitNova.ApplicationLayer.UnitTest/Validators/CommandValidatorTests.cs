using FluentAssertions;
using FluentValidation;
using Moq;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Features.Bundles.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Commands;
using TransitNova.BusinessLayer.Features.Shipments.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Validators.ShipmentValidators;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.ApplicationLayer.Tests.Validators;

public sealed class CommandValidatorTests
{
    [Fact]
    public async Task CreateBundleValidator_Should_AcceptCommand_When_UserAndDtoAreValidAsync()
    {
        var dtoValidator = CreateBundleDtoValidator();
        var validator = new CreateBundleCommandValidator(dtoValidator);
        var command = new CreateBundleCommand(Guid.NewGuid(), Guid.NewGuid(), ValidBundleDto());

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBundleValidator_Should_RejectCommand_When_UserIdIsEmptyAsync()
    {
        var validator = new CreateBundleCommandValidator(CreateBundleDtoValidator());
        var command = new CreateBundleCommand(Guid.NewGuid(), Guid.Empty, ValidBundleDto());

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBundleCommand.UserId));
    }

    [Theory]
    [InlineData("", 100, 10)]
    [InlineData("Bundle", 0, 10)]
    [InlineData("Bundle", 100, 0)]
    public async Task CreateBundleValidator_Should_RejectCommand_When_DtoBoundaryIsInvalidAsync(string name, decimal price, int shipments)
    {
        var validator = new CreateBundleCommandValidator(CreateBundleDtoValidator());
        var dto = ValidBundleDto();
        dto.BundleName = name;
        dto.BundlePrice = price;
        dto.TotalShipments = shipments;

        var result = await validator.ValidateAsync(new CreateBundleCommand(Guid.NewGuid(), Guid.NewGuid(), dto));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateCityValidator_Should_AcceptCommand_When_GovernmentExistsAndNameIsUniqueAsync()
    {
        var cities = new Mock<ICityRepository>();
        var countries = new Mock<ICountryRepository>();
        countries.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        cities.Setup(x => x.NameExistsForAnotherGovernmentAsync(1, "Cairo", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var validator = new CreateCityCommandValidator(CreateCityDtoValidator(), cities.Object, countries.Object);

        var result = await validator.ValidateAsync(
            new CreateCityCommand(Guid.NewGuid(), new CreateCityDto { Name = "Cairo", GovernmentId = 1 }));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCityValidator_Should_RejectCommand_When_GovernmentDoesNotExistAsync()
    {
        var cities = new Mock<ICityRepository>();
        var countries = new Mock<ICountryRepository>();
        countries.Setup(x => x.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        cities.Setup(x => x.NameExistsForAnotherGovernmentAsync(99, "Cairo", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var validator = new CreateCityCommandValidator(CreateCityDtoValidator(), cities.Object, countries.Object);

        var result = await validator.ValidateAsync(
            new CreateCityCommand(Guid.NewGuid(), new CreateCityDto { Name = "Cairo", GovernmentId = 99 }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Government not found.");
    }

    [Fact]
    public async Task CreateCityValidator_Should_RejectCommand_When_NameAlreadyExistsInGovernmentAsync()
    {
        var cities = new Mock<ICityRepository>();
        var countries = new Mock<ICountryRepository>();
        countries.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        cities.Setup(x => x.NameExistsForAnotherGovernmentAsync(1, "Cairo", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var validator = new CreateCityCommandValidator(CreateCityDtoValidator(), cities.Object, countries.Object);

        var result = await validator.ValidateAsync(
            new CreateCityCommand(Guid.NewGuid(), new CreateCityDto { Name = "Cairo", GovernmentId = 1 }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "City name already exists in this government.");
    }


    [Fact]
    public async Task RateCalculatorCommandValidator_Should_AcceptCommand_When_DtoIsValidAsync()
    {
        var validator = CreateRateCalculatorCommandValidator();
        var command = new RateCalculatorCommand(new RateCalculatorDto(
            ValidPackage(),
            TransportationMode.Land,
            enShipmentType.Standard));

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task RateCalculatorCommandValidator_Should_RejectCommand_When_DtoIsMissingAsync()
    {
        var validator = CreateRateCalculatorCommandValidator();
        var command = new RateCalculatorCommand(null!);

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RateCalculatorCommand.Dto));
    }
    private static InlineValidator<CreateBundleDto> CreateBundleDtoValidator()
    {
        var validator = new InlineValidator<CreateBundleDto>();
        validator.RuleFor(x => x.BundleName).NotEmpty();
        validator.RuleFor(x => x.BundlePrice).GreaterThan(0);
        validator.RuleFor(x => x.TotalWeight).GreaterThan(0);
        validator.RuleFor(x => x.TotalShipments).GreaterThan(0);
        return validator;
    }

    private static InlineValidator<CreateCityDto> CreateCityDtoValidator()
    {
        var validator = new InlineValidator<CreateCityDto>();
        validator.RuleFor(x => x.Name).NotEmpty();
        validator.RuleFor(x => x.GovernmentId).GreaterThan(0);
        return validator;
    }

    private static RateCalculatorCommandValidator CreateRateCalculatorCommandValidator() =>
        new(new RateCalculatorDtoValidator(new PackageSpecificationValidator()));

    private static PackageSpecificationDto ValidPackage() => new()
    {
        Weight = 5,
        Width = 10,
        Height = 10,
        Length = 10
    };
    private static CreateBundleDto ValidBundleDto() => new()
    {
        BundleName = "Business",
        BundlePrice = 100m,
        BundleDescription = "Business bundle",
        TotalWeight = 50m,
        TotalDistance = 200m,
        TotalShipments = 10
    };
}
