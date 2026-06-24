using FluentAssertions;
using Moq;
using TransitNova.BusinessLayer.Features.Token.Commands;
using TransitNova.BusinessLayer.Features.Token.Handler;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class RevokeRefreshTokenCommandHandlerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task RevokeRefreshTokenHandler_WhenRepositoryCompletes_ShouldReturnSuccessRegardlessOfAffectedRowsAsync(int affectedRows)
    {
        var repository = new Mock<IRefreshTokenRepository>();
        repository.Setup(x => x.RevokeAllUserTokenAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(affectedRows);
        var handler = new RevokeRefreshTokenHandler(repository.Object);

        var result = await handler.Handle(
            new RevokeRefreshTokenCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeRefreshTokenHandler_WhenCalled_ShouldRevokeTokensForCommandUserAsync()
    {
        var repository = new Mock<IRefreshTokenRepository>();
        var userId = Guid.NewGuid();
        var handler = new RevokeRefreshTokenHandler(repository.Object);

        await handler.Handle(new RevokeRefreshTokenCommand(Guid.NewGuid(), userId), CancellationToken.None);

        repository.Verify(x => x.RevokeAllUserTokenAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokenHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var repository = new Mock<IRefreshTokenRepository>();
        var handler = new RevokeRefreshTokenHandler(repository.Object);
        using var cancellation = new CancellationTokenSource();

        await handler.Handle(
            new RevokeRefreshTokenCommand(Guid.NewGuid(), Guid.NewGuid()),
            cancellation.Token);

        repository.Verify(x => x.RevokeAllUserTokenAsync(
            It.IsAny<Guid>(), cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokenHandler_WhenRepositoryFails_ShouldPropagateExceptionAsync()
    {
        var repository = new Mock<IRefreshTokenRepository>();
        repository.Setup(x => x.RevokeAllUserTokenAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("revoke failed"));
        var handler = new RevokeRefreshTokenHandler(repository.Object);

        var act = () => handler.Handle(
            new RevokeRefreshTokenCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("revoke failed");
    }
}
