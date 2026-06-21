using FluentAssertions;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Validators.UserProfile.Auth;
using TransitNova.BusinessLayer.Validators.VehicleValidators;
using TransitNova.BusinessLayer.Validators.WarehouseValidators;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.ApplicationLayer.Tests.Validators;

public sealed class DtoValidatorCoverageTests
{
    [Fact]
    public async Task CreateWarehouseDtoValidator_Should_AcceptDto_When_AllValuesAreValid()
    {
        var result = await new CreateWarehouseDtoValidator().ValidateAsync(ValidWarehouse());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("name")]
    [InlineData("address")]
    [InlineData("capacity")]
    [InlineData("usage")]
    [InlineData("hours")]
    [InlineData("type")]
    [InlineData("zones")]
    public async Task CreateWarehouseDtoValidator_Should_RejectDto_When_BusinessConstraintIsInvalid(string invalidField)
    {
        var dto = ValidWarehouse();
        switch (invalidField)
        {
            case "name": dto.Name = string.Empty; break;
            case "address": dto.Address = string.Empty; break;
            case "capacity": dto.Capacity = 0; break;
            case "usage": dto.CurrentUsage = dto.Capacity + 1; break;
            case "hours": dto.OperatingHours = 0; break;
            case "type": dto.Type = (WarehouseType)999; break;
            case "zones":
                var duplicate = Guid.NewGuid();
                dto.ZoneIds = [duplicate, duplicate];
                break;
        }

        var result = await new CreateWarehouseDtoValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateWarehouseDtoValidator_Should_AcceptNullOperatingHours_When_OtherValuesAreValid()
    {
        var dto = new UpdateWarehouseDto
        {
            Name = "Warehouse",
            Type = WarehouseType.MainWarehouse,
            Address = "Cairo",
            Capacity = 100,
            CurrentUsage = 20,
            OperatingHours = null,
            ZoneIds = []
        };

        var result = await new UpdateWarehouseDtoValidator().ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateWarehouseDtoValidator_Should_RejectDto_When_OperatingHoursIsZero()
    {
        var dto = new UpdateWarehouseDto
        {
            Name = "Warehouse",
            Type = WarehouseType.MainWarehouse,
            Address = "Cairo",
            Capacity = 100,
            CurrentUsage = 20,
            OperatingHours = 0,
            ZoneIds = []
        };

        var result = await new UpdateWarehouseDtoValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(UpdateWarehouseDto.OperatingHours));
    }

    [Fact]
    public async Task VehicleDtoValidator_Should_AcceptDto_When_AllValuesAreValid()
    {
        var result = await new VehicleDtoValidator().ValidateAsync(ValidVehicle());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("plate")]
    [InlineData("weight")]
    [InlineData("volume")]
    [InlineData("carrier")]
    [InlineData("type")]
    public async Task VehicleDtoValidator_Should_RejectDto_When_BusinessConstraintIsInvalid(string invalidField)
    {
        var dto = ValidVehicle();
        switch (invalidField)
        {
            case "plate": dto.PlateNumber = string.Empty; break;
            case "weight": dto.CapacityWeight = 0; break;
            case "volume": dto.CapacityVolume = 0; break;
            case "carrier": dto.CarrierId = Guid.Empty; break;
            case "type": dto.VehicleType = (VehicleType)999; break;
        }

        var result = await new VehicleDtoValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Password1!", "user@example.com", true)]
    [InlineData("", "user@example.com", false)]
    [InlineData("123", "user@example.com", false)]
    [InlineData("Password1!", "invalid-email", false)]
    [InlineData("Password1!", "", false)]
    public async Task LoginValidator_Should_ReturnExpectedValidity_When_CredentialsAreProvided(string password, string email, bool expected)
    {
        var result = await new LoginValidator().ValidateAsync(new LoginDto(password, email));

        result.IsValid.Should().Be(expected);
    }

    [Theory]
    [InlineData("OldPass1!", "NewPass1!", "NewPass1!", true)]
    [InlineData("", "NewPass1!", "NewPass1!", false)]
    [InlineData("OldPass1!", "short", "short", false)]
    [InlineData("OldPass1!", "NewPass1!", "Different1!", false)]
    public async Task ChangePasswordValidator_Should_ReturnExpectedValidity_When_PasswordsAreProvided(
        string current,
        string next,
        string confirmation,
        bool expected)
    {
        var result = await new ChangePasswordValidator().ValidateAsync(new ChangePasswordDto(current, next, confirmation));

        result.IsValid.Should().Be(expected);
    }

    [Fact]
    public async Task RegisterUserValidator_Should_AcceptDto_When_RegistrationDataIsStrongAndComplete()
    {
        var result = await new RegisterUserValidator().ValidateAsync(ValidRegistration());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("admin", "Strong9!Pass", "username")]
    [InlineData("valid_user", "Password123!", "password")]
    [InlineData("bad..name", "Strong9!Pass", "username")]
    [InlineData("valid_user", "weak", "password")]
    public async Task RegisterUserValidator_Should_RejectDto_When_CredentialRuleIsViolated(
        string username,
        string password,
        string expectedProperty)
    {
        var dto = ValidRegistration();
        dto.UserName = username;
        dto.Password = password;
        dto.ConfirmPassword = password;

        var result = await new RegisterUserValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName.Contains(expectedProperty, StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("invalid", "+201001234567", "Ahmed", "Ali", 1)]
    [InlineData("user@example.com", "000", "Ahmed", "Ali", 1)]
    [InlineData("user@example.com", "+201001234567", "A1", "Ali", 1)]
    [InlineData("user@example.com", "+201001234567", "Ahmed", "A2", 1)]
    [InlineData("user@example.com", "+201001234567", "Ahmed", "Ali", 0)]
    public async Task RegisterUserValidator_Should_RejectDto_When_ProfileFieldIsInvalid(
        string email,
        string phone,
        string firstName,
        string lastName,
        int cityId)
    {
        var dto = ValidRegistration();
        dto.Email = email;
        dto.PhoneNumber = phone;
        dto.FirstName = firstName;
        dto.LastName = lastName;
        dto.CityId = cityId;

        var result = await new RegisterUserValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
    }

    private static CreateWarehouseDto ValidWarehouse() => new()
    {
        Name = "Main Warehouse",
        Type = WarehouseType.MainWarehouse,
        Address = "Cairo",
        Capacity = 100,
        CurrentUsage = 20,
        OperatingHours = 12,
        ZoneIds = [Guid.NewGuid()]
    };

    private static VehicleDto ValidVehicle() => new()
    {
        VehicleType = VehicleType.Van,
        PlateNumber = "ABC-123",
        CapacityWeight = 1000,
        CapacityVolume = 50,
        CarrierId = Guid.NewGuid()
    };

    private static RegisterDto ValidRegistration() => new()
    {
        UserName = "ahmed_user",
        Email = "ahmed@example.com",
        Password = "Strong9!Pass",
        ConfirmPassword = "Strong9!Pass",
        PhoneNumber = "+201001234567",
        FirstName = "Ahmed",
        LastName = "Ali",
        Address = "Cairo",
        UserType = UserType.User,
        CityId = 1
    };
}
