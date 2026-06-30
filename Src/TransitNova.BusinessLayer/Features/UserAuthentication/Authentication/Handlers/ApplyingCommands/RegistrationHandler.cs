using AutoMapper;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands
{
    public sealed class RegistrationHandler
        (
         IUserAuthCommandsService authRepository,
         IUserAuthQueryService userQuery,
         IRefreshTokenRepository tokenRepo,
         ISystemLogCommands systemLogCommands,
         IUnitOfWork unitOfWork,
         IMapper mapper,
         ITokenProvider tokenGenerator,
         IUserStrategyFactory factory,
         ILogger<RegistrationHandler> logger
        ) : ICommandHandler<RegistrationCommand, Result<AuthResponseDto>>
    {

        public async Task<Result<AuthResponseDto>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Register For User Whose Name => {Name} Started", request.Dto.FirstName);


            //======== Phase 1 Resolve User Strategy And Create User =============


            var user = mapper.Map<AppUserDto>(request.Dto);

            //======== Phase 2 Identity Operations =============

            //======= Create User =======
            var createdUser = await authRepository.CreateUserAsync(user, request.Dto.Password, cancellationToken);
           
            //======= Try Adding To Role =======
            await authRepository.AddToRoleAsync(createdUser, request.Dto.UserType.ToString(), cancellationToken);


            //====== Creating User Profile By Strategy
            var registerStrategy = factory.ResolveUserStrategy(request.Dto.UserType);
            if (registerStrategy is null)
                return Result<AuthResponseDto>.Failure(Errors.FailedOperation("User Type Is Unavailable."));

            await registerStrategy.ExecuteAsync(createdUser.Id, request.Dto, cancellationToken);


            //======== Generate Refresh Token ========
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            await tokenRepo.AddRefreshTokenAsync(createdUser.Id, refreshToken, cancellationToken);

            var activityEntityType = request.Dto.UserType switch
            {
                UserType.Carrier => ActivityEntityType.Carrier,
                UserType.OperationManager => ActivityEntityType.OperationManager,
                _ => ActivityEntityType.User
            };
            var performedByName = createdUser.FullName!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                activityEntityType,
                $"{request.Dto.UserType} {performedByName} registered successfully.",
                createdUser.Id,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            //===== Saving Changes

            await unitOfWork.SaveChangesAsync(cancellationToken);
            //===== Getting User Roles
            var roles = await userQuery.GetUserRolesAsync(createdUser.Id, cancellationToken);
            createdUser.Roles = [.. roles];

            //====== Generating Access Token 
            var token = await tokenGenerator.GenerateTokenAsync(createdUser);

            //====Building Response
            var authResponseBuilder = new AuthResponseDto
            {
                IsAuthenticated = true,
                Id = createdUser.Id,
                Email = createdUser.Email,
                Username = createdUser.UserName,
                Token = token,
                RefreshToken = refreshToken,
                UserType = request.Dto.UserType.ToString(),
                Roles = [.. roles]
            };
            logger.LogInformation("User {Email} registered successfully with Id: {UserId}", createdUser.Email, createdUser.Id);
            return Result<AuthResponseDto>.Success(authResponseBuilder);
        }
    }
}

