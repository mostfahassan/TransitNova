using FluentAssertions;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Validators.UserProfile.Auth;

namespace TransitNova.ApplicationLayer.Tests.Validators.Auth;

public sealed class LoginCommandValidatorTests
{
    [Theory]
    [InlineData("", "Strong9!Pass", false)]
    [InlineData("invalid", "Strong9!Pass", false)]
    [InlineData("user@example.com", "", false)]
    [InlineData("user@example.com", "123", false)]
    [InlineData("user@example.com", "Strong9!Pass", true)]
    public async Task LoginCommandValidator_WhenCredentialsAreProvided_ShouldReturnExpectedValidityAsync(
        string email,
        string password,
        bool expectedValidity)
    {
        var validator = new LoginCommandValidator(new LoginValidator());
        var command = new LoginCommand( new LoginDto(password, email));

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().Be(expectedValidity);
    }
}
