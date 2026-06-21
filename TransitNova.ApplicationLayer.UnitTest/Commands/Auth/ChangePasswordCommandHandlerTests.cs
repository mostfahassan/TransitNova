using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class ChangePasswordCommandHandlerTests
{
    [Fact]
    public async Task ChangePasswordHandler_WhenIdentityAcceptsPasswordChange_ShouldReturnSuccess()
    {
        var fixture = new ChangePasswordFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordHandler_WhenCalled_ShouldUseCommandUserId()
    {
        var fixture = new ChangePasswordFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Auth.Verify(x => x.ChangePasswordAsync(
            fixture.Command.AppUserId.ToString(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordHandler_WhenCalled_ShouldForwardCurrentPassword()
    {
        var fixture = new ChangePasswordFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Auth.Verify(x => x.ChangePasswordAsync(
            It.IsAny<string>(), fixture.Dto.CurrentPassword, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordHandler_WhenCalled_ShouldForwardNewPassword()
    {
        var fixture = new ChangePasswordFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Auth.Verify(x => x.ChangePasswordAsync(
            It.IsAny<string>(), It.IsAny<string>(), fixture.Dto.NewPassword, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new ChangePasswordFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Auth.Verify(x => x.ChangePasswordAsync(
            fixture.Command.AppUserId.ToString(),
            fixture.Dto.CurrentPassword,
            fixture.Dto.NewPassword,
            cancellation.Token), Times.Once);
    }

    [Theory]
    [InlineData("wrong current password")]
    [InlineData("user not found")]
    [InlineData("password policy rejected")]
    public async Task ChangePasswordHandler_WhenIdentityServiceRejectsChange_ShouldPropagateWithoutSuccess(string message)
    {
        var fixture = new ChangePasswordFixture();
        fixture.Auth.Setup(x => x.ChangePasswordAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(message));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(message);
    }

    private sealed class ChangePasswordFixture
    {
        public ChangePasswordDto Dto { get; } = new("OldPass1!", "NewPass2!", "NewPass2!");
        public ChangePasswordCommand Command { get; }
        public Mock<IUserAuthCommandsService> Auth { get; } = new();
        public ChangePasswordHandler Handler { get; }

        public ChangePasswordFixture()
        {
            Command = new ChangePasswordCommand(Guid.NewGuid(), Dto, Guid.NewGuid());
            Handler = new ChangePasswordHandler(Mock.Of<ILogger<ChangePasswordHandler>>(), Auth.Object);
        }
    }
}
