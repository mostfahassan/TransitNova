using FluentAssertions;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Validators.UserProfile.Auth;

namespace TransitNova.ApplicationLayer.Tests.Validators.Auth;

public sealed class ChangePasswordCommandValidatorTests
{
    [Theory]
    [InlineData("OldPass1!", "NewPass2!", "NewPass2!", true, true)]
    [InlineData("", "NewPass2!", "NewPass2!", true, false)]
    [InlineData("OldPass1!", "short", "short", true, false)]
    [InlineData("OldPass1!", "NewPass2!", "Different3!", true, false)]
    [InlineData("OldPass1!", "NewPass2!", "NewPass2!", false, false)]
    public async Task ChangePasswordCommandValidator_WhenInputVaries_ShouldReturnExpectedValidityAsync(
        string current,
        string next,
        string confirmation,
        bool hasUserId,
        bool expectedValidity)
    {
        var validator = new ChangePasswordCommandValidator(new ChangePasswordValidator());
        var command = new ChangePasswordCommand(
        
            new ChangePasswordDto(current, next, confirmation),
            hasUserId ? Guid.NewGuid() : Guid.Empty);

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().Be(expectedValidity);
    }
}
