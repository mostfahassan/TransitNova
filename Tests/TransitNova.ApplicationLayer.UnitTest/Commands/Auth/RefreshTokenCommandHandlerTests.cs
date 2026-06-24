using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Features.Token.Commands;
using TransitNova.BusinessLayer.Features.Token.Handler;
using TransitNova.BusinessLayer.Interfaces.Services.TokenServices;
using TransitNova.BusinessLayer.Services.TokenServices;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task GenerateRefreshTokenHandler_WhenServiceReturnsTokens_ShouldReturnAuthenticatedResponseAsync()
    {
        var fixture = new RefreshFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateRefreshTokenHandler_WhenServiceReturnsTokens_ShouldReturnRotatedTokensAsync()
    {
        var fixture = new RefreshFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Data!.Token.Should().Be(fixture.Generated.AccessToken);
        result.Data.RefreshToken.Should().Be(fixture.Generated.RefreshToken);
    }

    [Fact]
    public async Task GenerateRefreshTokenHandler_WhenServiceReturnsTokens_ShouldReturnUserAndRolesAsync()
    {
        var fixture = new RefreshFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Data!.Id.Should().Be(fixture.User.Id);
        result.Data.UserType.Should().Be(UserType.Carrier.ToString());
        result.Data.Roles.Should().Equal("Carrier", "Dispatcher");
    }

    [Fact]
    public async Task GenerateRefreshTokenHandler_WhenCalled_ShouldForwardTokenAndCancellationAsync()
    {
        var fixture = new RefreshFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Service.Verify(x => x.GetNewTokenAsync(
            fixture.Command.RefreshToken,
            cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task GenerateRefreshTokenHandler_WhenTokenServiceFails_ShouldPropagateExceptionAsync()
    {
        var fixture = new RefreshFixture();
        fixture.Service.Setup(x => x.GetNewTokenAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("invalid refresh token"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("invalid refresh token");
    }

    private sealed class RefreshFixture
    {
        public AppUserDto User { get; } = new()
        {
            Id = Guid.NewGuid(),
            UserType = UserType.Carrier,
            Email = "carrier@example.com",
            UserName = "carrier"
        };
        public GenerateRefreshTokenCommand Command { get; } = new(Guid.NewGuid(), "old-refresh-token");
        public GeneratedTokenResult Generated { get; }
        public Mock<ITokenService> Service { get; } = new();
        public GenerateRefreshTokenHandler Handler { get; }

        public RefreshFixture()
        {
            Generated = new GeneratedTokenResult
            {
                ValidToken = new RefreshTokenDto
                {
                    Token = Command.RefreshToken,
                    UserId = User.Id,
                    User = User,
                    ExpiresOn = DateTime.UtcNow.AddDays(1)
                },
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token",
                Roles = ["Carrier", "Dispatcher"]
            };
            Service.Setup(x => x.GetNewTokenAsync(
                    Command.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Generated);
            Handler = new GenerateRefreshTokenHandler(
                Service.Object,
                Mock.Of<ILogger<GenerateRefreshTokenHandler>>());
        }
    }
}
