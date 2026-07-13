using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class LogoutCommandHandlerTests
{
    [Fact]
    public async Task SignOutHandler_WhenCalled_ShouldReturnSuccessAsync()
    {
        var tokens = new Mock<IRefreshTokenRepository>();
        var handler = CreateHandler(tokens);

        var result = await handler.Handle(new SignOutCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SignOutHandler_WhenCalled_ShouldRevokeAllRefreshTokensForCurrentUserAsync()
    {
        var userId = Guid.NewGuid();
        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(x => x.RevokeAllUserTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        var handler = CreateHandler(tokens);

        await handler.Handle(new SignOutCommand(userId), CancellationToken.None);

        tokens.Verify(x => x.RevokeAllUserTokenAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SignOutHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var userId = Guid.NewGuid();
        var tokens = new Mock<IRefreshTokenRepository>();
        var handler = CreateHandler(tokens);
        using var cancellation = new CancellationTokenSource();

        await handler.Handle(new SignOutCommand(userId), cancellation.Token);

        tokens.Verify(x => x.RevokeAllUserTokenAsync(userId, cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task SignOutHandler_WhenTokenRevocationFails_ShouldPropagateExceptionAsync()
    {
        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(x => x.RevokeAllUserTokenAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("token revocation failed"));
        var handler = CreateHandler(tokens);

        var act = () => handler.Handle(new SignOutCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("token revocation failed");
    }

    private static SignOutHandler CreateHandler(Mock<IRefreshTokenRepository> tokens) =>
        new(Mock.Of<ILogger<SignOutHandler>>(), tokens.Object);
}
