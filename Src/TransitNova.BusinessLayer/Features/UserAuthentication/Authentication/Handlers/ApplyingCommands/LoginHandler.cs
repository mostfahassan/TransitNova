
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Token;

using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands
{
    public sealed class LoginHandler(
        IUserAuthQueryService authRepository,
        IUserAuthRulesService userRules,
        IRefreshTokenRepository tokenRepo,
        ILogger<LoginHandler> logger,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenGenerator)
        : ICommandHandler<LoginCommand, Result<AuthResponseDto>>
    {
        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var (valid,user ,error) = await userRules.ValidatePasswordAsync(request.Dto.Email, request.Dto.Password, lockoutOnFailure :true,
                cancellationToken);
            if (!valid || user is null)
            {
                logger.LogWarning("Login failed for {Email}. Reason: {Reason}", request.Dto.Email, error);
                return Result<AuthResponseDto>.Failure(Errors.InvalidCredentials("Invalid email or password."));
            }

            //BuildAsync Refresh Token
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            await tokenRepo.AddRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            
            // Token Generation
            var roles = await authRepository.GetUserRolesAsync(user.Id, cancellationToken);
            user.Roles = [..roles];
            var token = await tokenGenerator.GenerateTokenAsync(user);
          
            var authResponseBuilder = new AuthResponseDto
            {
                IsAuthenticated = true,
                Email = user.Email,
                Id = user.Id,
                Username = user.UserName,
                Token = token,
                RefreshToken = refreshToken,
                UserType = user.UserType.ToString(),
                Roles = [.. roles]
            };
            logger.LogInformation("User {Email} logged in successfully. Id: {UserId}", user.Email, user.Id);

            return Result<AuthResponseDto>.Success(authResponseBuilder);
        }
    }
}
