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

namespace TransitNova.ApplicationLayer.Tests.Validators;

public sealed class CommandValidatorTests
{
    [Fact]
    public async Task CreateBundleValidator_Should_AcceptCommand_When_UserAndDtoAreValid()
    {
        var dtoValidator = CreateBundleDtoValidator();
        var validator = new CreateBundleCommandValidator(dtoValidator);
        var command = new CreateBundleCommand(Guid.NewGuid(), Guid.NewGuid(), ValidBundleDto());

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBundleValidator_Should_RejectCommand_When_UserIdIsEmpty()
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
    public async Task CreateBundleValidator_Should_RejectCommand_When_DtoBoundaryIsInvalid(string name, decimal price, int shipments)
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
    public async Task CreateCityValidator_Should_AcceptCommand_When_GovernmentExistsAndNameIsUnique()
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
    public async Task CreateCityValidator_Should_RejectCommand_When_GovernmentDoesNotExist()
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
    public async Task CreateCityValidator_Should_RejectCommand_When_NameAlreadyExistsInGovernment()
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
