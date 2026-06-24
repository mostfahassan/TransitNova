using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldReturnAuthenticatedResponseAsync()
    {
        var fixture = new LoginFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldReturnAccessAndRefreshTokensAsync()
    {
        var fixture = new LoginFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Data!.Token.Should().Be(fixture.AccessToken);
        result.Data.RefreshToken.Should().Be(fixture.RefreshToken);
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldReturnCorrectUserIdentityAsync()
    {
        var fixture = new LoginFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Data!.Id.Should().Be(fixture.User.Id);
        result.Data.Email.Should().Be(fixture.User.Email);
        result.Data.Username.Should().Be(fixture.User.UserName);
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldPersistRefreshTokenAsync()
    {
        var fixture = new LoginFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(
            fixture.User.Id,
            fixture.RefreshToken,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldSaveBeforeReturningAsync()
    {
        var fixture = new LoginFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreValid_ShouldLoadRolesBeforeGeneratingTokenAsync()
    {
        var fixture = new LoginFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.TokenProvider.Verify(x => x.GenerateTokenAsync(
            It.Is<AppUserDto>(user => user.Roles.SequenceEqual(fixture.Roles))), Times.Once);
        result.Data!.Roles.Should().Equal(fixture.Roles);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task LoginHandler_WhenValidationDoesNotProduceValidUser_ShouldReturnSameInvalidCredentialsErrorAsync(
        bool valid,
        bool returnNullUser)
    {
        var fixture = new LoginFixture();
        fixture.Rules.Setup(x => x.ValidatePasswordAsync(
                fixture.Dto.Email,
                fixture.Dto.Password,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((valid, returnNullUser ? null : fixture.User, "identity detail"));

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Invalid email or password.");
        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginHandler_WhenCredentialsAreInvalid_ShouldNotGenerateTokensAsync()
    {
        var fixture = new LoginFixture();
        fixture.Rules.Setup(x => x.ValidatePasswordAsync(
                It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (AppUserDto?)null, "bad password"));

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.TokenProvider.Verify(x => x.GenerateRefreshToken(), Times.Never);
        fixture.TokenProvider.Verify(x => x.GenerateTokenAsync(It.IsAny<AppUserDto>()), Times.Never);
    }

    [Fact]
    public async Task LoginHandler_WhenRefreshTokenPersistenceFails_ShouldPropagateWithoutSavingAsync()
    {
        var fixture = new LoginFixture();
        fixture.Tokens.Setup(x => x.AddRefreshTokenAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("token persistence failed"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("token persistence failed");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginHandler_WhenCancellationTokenIsPassed_ShouldForwardItToDependenciesAsync()
    {
        var fixture = new LoginFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Rules.Verify(x => x.ValidatePasswordAsync(
            fixture.Dto.Email, fixture.Dto.Password, true, cancellation.Token), Times.Once);
        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(
            fixture.User.Id, fixture.RefreshToken, cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    private sealed class LoginFixture
    {
        public LoginDto Dto { get; } = new("Strong9!Pass", "mona@example.com");
        public LoginCommand Command { get; }
        public AppUserDto User { get; } = new()
        {
            Id = Guid.NewGuid(),
            Email = "mona@example.com",
            UserName = "mona_user",
            FullName = "Mona Ali",
            UserType = UserType.User
        };
        public string RefreshToken { get; } = "refresh-token";
        public string AccessToken { get; } = "access-token";
        public string[] Roles { get; } = ["User"];
        public Mock<IUserAuthQueryService> Users { get; } = new();
        public Mock<IUserAuthRulesService> Rules { get; } = new();
        public Mock<IRefreshTokenRepository> Tokens { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ITokenProvider> TokenProvider { get; } = new();
        public LoginHandler Handler { get; }

        public LoginFixture()
        {
            Command = new LoginCommand(Dto);
            Rules.Setup(x => x.ValidatePasswordAsync(
                    Dto.Email, Dto.Password, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, User, string.Empty));
            TokenProvider.Setup(x => x.GenerateRefreshToken()).Returns(RefreshToken);
            TokenProvider.Setup(x => x.GenerateTokenAsync(It.IsAny<AppUserDto>())).ReturnsAsync(AccessToken);
            Users.Setup(x => x.GetUserRolesAsync(User.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Roles);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new LoginHandler(
                Users.Object,
                Rules.Object,
                Tokens.Object,
                Mock.Of<ILogger<LoginHandler>>(),
                UnitOfWork.Object,
                TokenProvider.Object);
        }
    }
}
