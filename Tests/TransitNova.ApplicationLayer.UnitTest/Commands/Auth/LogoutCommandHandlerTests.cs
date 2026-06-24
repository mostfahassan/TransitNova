using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class LogoutCommandHandlerTests
{
    [Fact]
    public async Task SignOutHandler_WhenCalled_ShouldReturnSuccessAsync()
    {
        var rules = new Mock<IUserAuthRulesService>();
        var handler = CreateHandler(rules);

        var result = await handler.Handle(new SignOutCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SignOutHandler_WhenCalled_ShouldSignOutExactlyOnceAsync()
    {
        var rules = new Mock<IUserAuthRulesService>();
        var handler = CreateHandler(rules);

        await handler.Handle(new SignOutCommand(), CancellationToken.None);

        rules.Verify(x => x.SignOutAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SignOutHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var rules = new Mock<IUserAuthRulesService>();
        var handler = CreateHandler(rules);
        using var cancellation = new CancellationTokenSource();

        await handler.Handle(new SignOutCommand(), cancellation.Token);

        rules.Verify(x => x.SignOutAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task SignOutHandler_WhenIdentitySignOutFails_ShouldPropagateExceptionAsync()
    {
        var rules = new Mock<IUserAuthRulesService>();
        rules.Setup(x => x.SignOutAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("sign out failed"));
        var handler = CreateHandler(rules);

        var act = () => handler.Handle(new SignOutCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("sign out failed");
    }

    private static SignOutHandler CreateHandler(Mock<IUserAuthRulesService> rules) =>
        new(Mock.Of<ILogger<SignOutHandler>>(), rules.Object);
}
