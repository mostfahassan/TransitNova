using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.ApplicationLayer.Tests.Commands.Auth;

public sealed class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldReturnAuthenticatedResponseAsync()
    {
        var fixture = new RegistrationFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldMapRegistrationDtoAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Mapper.Verify(x => x.Map<AppUserDto>(fixture.Dto), Times.Once);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldCreateIdentityUserWithProvidedPasswordAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.AuthCommands.Verify(x => x.CreateUserAsync(
            fixture.MappedUser,
            fixture.Dto.Password,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldAssignRequestedRoleAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.AuthCommands.Verify(x => x.AddToRoleAsync(
            fixture.CreatedUser,
            UserType.User.ToString(),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldExecuteResolvedProfileStrategyAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Strategy.Verify(x => x.ExecuteAsync(
            fixture.CreatedUser.Id,
            fixture.Dto,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldPersistGeneratedRefreshTokenAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(
            fixture.CreatedUser.Id,
            fixture.RefreshToken,
            CancellationToken.None), Times.Once);
    }

    [Theory]
    [InlineData(UserType.User, ActivityEntityType.User)]
    [InlineData(UserType.Carrier, ActivityEntityType.Carrier)]
    [InlineData(UserType.OperationManager, ActivityEntityType.OperationManager)]
    public async Task RegistrationHandler_WhenRegistrationSucceeds_ShouldLogCorrectBusinessEntityAsync(
        UserType userType,
        ActivityEntityType expectedEntityType)
    {
        var fixture = new RegistrationFixture(userType);
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.EntityType.Should().Be(expectedEntityType);
        captured.Action.Should().Be(ActivityAction.Created);
        captured.PerformedByName.Should().Be(fixture.CreatedUser.FullName);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldSaveChangesOnceAsync()
    {
        var fixture = new RegistrationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RegistrationHandler_WhenRegistrationIsValid_ShouldGenerateAccessTokenAfterRolesAreLoadedAsync()
    {
        var fixture = new RegistrationFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.TokenProvider.Verify(x => x.GenerateTokenAsync(
            It.Is<AppUserDto>(user => user.Roles.SequenceEqual(fixture.Roles))), Times.Once);
        result.Data!.Token.Should().Be(fixture.AccessToken);
        result.Data.Roles.Should().Equal(fixture.Roles);
    }

    [Fact]
    public async Task RegistrationHandler_WhenUserTypeHasNoStrategy_ShouldReturnFailureWithoutSavingAsync()
    {
        var fixture = new RegistrationFixture();
        fixture.Factory.Setup(x => x.ResolveUserStrategy(fixture.Dto.UserType))
            .Returns((IUserRegistrationStrategy?)null);

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Tokens.Verify(x => x.AddRefreshTokenAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrationHandler_WhenIdentityCreationFails_ShouldPropagateWithoutSavingAsync()
    {
        var fixture = new RegistrationFixture();
        fixture.AuthCommands.Setup(x => x.CreateUserAsync(
                It.IsAny<AppUserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("identity creation failed"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("identity creation failed");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private sealed class RegistrationFixture
    {
        public RegisterDto Dto { get; }
        public RegistrationCommand Command { get; }
        public AppUserDto MappedUser { get; }
        public AppUserDto CreatedUser { get; }
        public string RefreshToken { get; } = "refresh-token";
        public string AccessToken { get; } = "access-token";
        public string[] Roles { get; } = ["User"];
        public Mock<IUserAuthCommandsService> AuthCommands { get; } = new();
        public Mock<IUserAuthQueryService> Users { get; } = new();
        public Mock<IRefreshTokenRepository> Tokens { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IMapper> Mapper { get; } = new();
        public Mock<ITokenProvider> TokenProvider { get; } = new();
        public Mock<IUserStrategyFactory> Factory { get; } = new();
        public Mock<IUserRegistrationStrategy> Strategy { get; } = new();
        public RegistrationHandler Handler { get; }

        public RegistrationFixture(UserType userType = UserType.User)
        {
            Dto = new RegisterDto
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
                UserType = userType
            };
            Command = new RegistrationCommand(Dto);
            MappedUser = new AppUserDto
            {
                Id = Guid.NewGuid(),
                UserName = Dto.UserName,
                Email = Dto.Email,
                FullName = "Mona Ali",
                UserType = userType
            };
            CreatedUser = new AppUserDto
            {
                Id = Guid.NewGuid(),
                UserName = Dto.UserName,
                Email = Dto.Email,
                FullName = "Mona Ali",
                UserType = userType
            };
            Mapper.Setup(x => x.Map<AppUserDto>(Dto)).Returns(MappedUser);
            AuthCommands.Setup(x => x.CreateUserAsync(MappedUser, Dto.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatedUser);
            AuthCommands.Setup(x => x.AddToRoleAsync(
                    CreatedUser, userType.ToString(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            Strategy.SetupGet(x => x.UserType).Returns(userType);
            Strategy.Setup(x => x.ExecuteAsync(
                    CreatedUser.Id, Dto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            Factory.Setup(x => x.ResolveUserStrategy(userType)).Returns(Strategy.Object);
            TokenProvider.Setup(x => x.GenerateRefreshToken()).Returns(RefreshToken);
            TokenProvider.Setup(x => x.GenerateTokenAsync(It.IsAny<AppUserDto>())).ReturnsAsync(AccessToken);
            Users.Setup(x => x.GetUserRolesAsync(CreatedUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Roles);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new RegistrationHandler(
                AuthCommands.Object,
                Users.Object,
                Tokens.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mapper.Object,
                TokenProvider.Object,
                Factory.Object,
                Mock.Of<ILogger<RegistrationHandler>>());
        }
    }
}
