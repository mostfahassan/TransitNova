using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.Exceptions;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Features.Token.Handler;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.BusinessLayer.Services.TokenServices;
using TransitNova.Domain.DomainExceptions;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class TokenServiceTests
{
    [Fact]
    public async Task GetNewTokenAsync_Should_ThrowNotFound_When_RefreshTokenDoesNotExist()
    {
        var fixture = new Fixture();
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("missing", It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenDto?)null);

        var act = () => fixture.Service.GetNewTokenAsync("missing", CancellationToken.None);

        await act.Should().ThrowAsync<RefreshTokenNotFoundException>();
        fixture.Provider.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetNewTokenAsync_Should_ThrowInvalidToken_When_RefreshTokenIsExpired()
    {
        var fixture = new Fixture();
        var token = ValidToken();
        token.ExpiresOn = DateTime.UtcNow.AddMinutes(-1);
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("expired", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var act = () => fixture.Service.GetNewTokenAsync("expired", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRefreshTokenException>();
        fixture.Provider.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetNewTokenAsync_Should_ThrowNotFound_When_TokenUserIsMissing()
    {
        var fixture = new Fixture();
        var token = ValidToken();
        token.User = null;
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("orphan", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var act = () => fixture.Service.GetNewTokenAsync("orphan", CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>().Where(x => x.ErrorCode == "USER_NOT_FOUND");
    }

    [Fact]
    public async Task GetNewTokenAsync_Should_RevokeAllAndThrowReuseException_When_TokenWasRevoked()
    {
        var fixture = new Fixture();
        var token = ValidToken();
        token.IsRevoked = true;
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("reused", It.IsAny<CancellationToken>())).ReturnsAsync(token);
        fixture.Tokens.Setup(x => x.RevokeAllUserTokenAsync(token.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var act = () => fixture.Service.GetNewTokenAsync("reused", CancellationToken.None);

        await act.Should().ThrowAsync<Domain.DomainExceptions.ReusedRefreshTokenException>();
        fixture.Tokens.Verify(x => x.RevokeAllUserTokenAsync(token.UserId, CancellationToken.None), Times.Once);
        fixture.Provider.Verify(x => x.GenerateRefreshToken(), Times.Never);
    }

    [Fact]
    public async Task GetNewTokenAsync_Should_ThrowRevokingExceptionAndSkipPersistence_When_OldTokenCannotBeRevoked()
    {
        var fixture = new Fixture();
        var token = ValidToken();
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(token);
        fixture.Provider.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh");
        fixture.Tokens.Setup(x => x.RevokeOldRefreshTokenAsync(token.UserId, token.Token, "new-refresh", It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var act = () => fixture.Service.GetNewTokenAsync("old", CancellationToken.None);

        await act.Should().ThrowAsync<BusinessLayer.Common.Exceptions.ReusedRefreshTokenException.RevokingRefreshTokenException>();
        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetNewTokenAsync_Should_RotatePersistAndReturnTokens_When_RefreshTokenIsValid()
    {
        var fixture = new Fixture();
        var token = ValidToken();
        fixture.Tokens.Setup(x => x.GetRefreshTokenAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(token);
        fixture.Provider.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh");
        fixture.Tokens.Setup(x => x.RevokeOldRefreshTokenAsync(token.UserId, token.Token, "new-refresh", It.IsAny<CancellationToken>())).ReturnsAsync(1);
        fixture.Roles.Setup(x => x.GetUserRolesAsync(token.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(["User", "Subscriber"]);
        fixture.Provider.Setup(x => x.GenerateToken(token.User!)).ReturnsAsync("new-access");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await fixture.Service.GetNewTokenAsync("old", CancellationToken.None);

        result.ValidToken.Should().BeSameAs(token);
        result.RefreshToken.Should().Be("new-refresh");
        result.AccessToken.Should().Be("new-access");
        result.Roles.Should().Equal("User", "Subscriber");
        token.User!.Roles.Should().Equal("User", "Subscriber");
        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(token.UserId, "new-refresh", CancellationToken.None), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    private static RefreshTokenDto ValidToken() => new()
    {
        Token = "old-token",
        UserId = Guid.NewGuid(),
        ExpiresOn = DateTime.UtcNow.AddHours(1),
        User = new AppUserDto
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user"
        }
    };

    private sealed class Fixture
    {
        internal Mock<IRefreshTokenRepository> Tokens { get; } = new();
        internal Mock<ITokenProvider> Provider { get; } = new();
        internal Mock<IUserAuthQueryService> Roles { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal TokenService Service { get; }

        internal Fixture()
        {
            Service = new TokenService(
                Tokens.Object,
                Mock.Of<ILogger<GenerateRefreshTokenHandler>>(),
                Provider.Object,
                Roles.Object,
                UnitOfWork.Object);
        }
    }
}
