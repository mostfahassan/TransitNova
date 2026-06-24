using FluentAssertions;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Validators.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.ApplicationLayer.Tests.Validators.Auth;

public sealed class RegisterUserCommandValidatorTests
{
    [Fact]
    public async Task RegistrationCommandValidator_WhenAllFieldsAreValid_ShouldPassValidationAsync()
    {
        var result = await CreateValidator().ValidateAsync(Command(ValidDto()));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("admin")]
    [InlineData("bad..name")]
    [InlineData("bad_name_")]
    [InlineData("1invalid")]
    public async Task RegistrationCommandValidator_WhenUsernameViolatesPolicy_ShouldFailValidationAsync(string username)
    {
        var dto = ValidDto();
        dto.UserName = username;

        var result = await CreateValidator().ValidateAsync(Command(dto));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName.EndsWith(nameof(RegisterDto.UserName)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    public async Task RegistrationCommandValidator_WhenEmailIsInvalid_ShouldFailValidationAsync(string email)
    {
        var dto = ValidDto();
        dto.Email = email;

        var result = await CreateValidator().ValidateAsync(Command(dto));

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("nouppercase9!")]
    [InlineData("NOLOWERCASE9!")]
    [InlineData("NoNumber!")]
    [InlineData("NoSpecial9")]
    [InlineData("With Space9!")]
    [InlineData("Password123!")]
    public async Task RegistrationCommandValidator_WhenPasswordViolatesSecurityPolicy_ShouldFailValidationAsync(string password)
    {
        var dto = ValidDto();
        dto.Password = password;
        dto.ConfirmPassword = password;

        var result = await CreateValidator().ValidateAsync(Command(dto));

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("000")]
    [InlineData("+000000")]
    [InlineData("1234567890123456")]
    public async Task RegistrationCommandValidator_WhenPhoneIsInvalid_ShouldFailValidationAsync(string phone)
    {
        var dto = ValidDto();
        dto.PhoneNumber = phone;

        var result = await CreateValidator().ValidateAsync(Command(dto));

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("first")]
    [InlineData("last")]
    [InlineData("city")]
    [InlineData("confirmation")]
    public async Task RegistrationCommandValidator_WhenRequiredProfileValueIsInvalid_ShouldFailValidationAsync(string field)
    {
        var dto = ValidDto();
        switch (field)
        {
            case "first": dto.FirstName = "1"; break;
            case "last": dto.LastName = string.Empty; break;
            case "city": dto.CityId = 0; break;
            case "confirmation": dto.ConfirmPassword = "Different9!Pass"; break;
        }

        var result = await CreateValidator().ValidateAsync(Command(dto));

        result.IsValid.Should().BeFalse();
    }

    private static RegistrationCommandValidator CreateValidator() => new(new RegisterUserValidator());
    private static RegistrationCommand Command(RegisterDto dto) => new(dto);
    private static RegisterDto ValidDto() => new()
    {
        UserName = "mona_user",
        Email = "mona@example.com",
        Password = "Strong9!Pass",
        ConfirmPassword = "Strong9!Pass",
        PhoneNumber = "+201001234567",
        FirstName = "Mona",
        LastName = "Ali",
        Address = "Cairo",
        CityId = 1,
        UserType = UserType.User
    };
}
