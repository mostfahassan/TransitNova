using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;

namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public sealed class GetAdminUserDetailsHandler(
        IUserQueryRepository userQueryRepository,
        ILogger<GetAdminUserDetailsHandler> logger)
        : IQueryHandler<GetAdminUserDetailsQuery, Result<AdminUserDetailsDto>>
    {
        public async Task<Result<AdminUserDetailsDto>> Handle(GetAdminUserDetailsQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving user details for admin. UserId: {UserId}", request.UserId);

            var user = await userQueryRepository.GetUserDetailsForAdminAsync(request.UserId, ct);
            if (user is null)
            {
                logger.LogWarning("User details not found for admin request. UserId: {UserId}", request.UserId);
                var notFoundResult = Result<AdminUserDetailsDto>.NotFound(Errors.UserNotFound($"User with Id '{request.UserId}' was not found."));
                return notFoundResult;
            }

            logger.LogInformation("User details retrieved successfully for admin. UserId: {UserId}", request.UserId);
            var result = Result<AdminUserDetailsDto>.Success(user);
            return result;
        }
    }
}
