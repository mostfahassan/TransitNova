
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public sealed class GetUserProfileHandler(IUserQueryRepository userQuery, ILogger<GetUserProfileHandler> logger)
    : IQueryHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving user profile for UserId: {UserId}", request.Id);

            var userProfile = await userQuery.GetUserProfileAsync(request.Id, cancellationToken);

            if (userProfile is null)
            {
                logger.LogWarning("User profile not found for UserId: {UserId}", request.Id);

                var notFoundResult = Result<UserProfileDto>.Failure(
                    Errors.UserNotFound($"User with Id '{request.Id}' was not found."));
                return notFoundResult;
            }
            logger.LogInformation("Successfully retrieved user profile for UserId: {UserId}", request.Id);
            var result = Result<UserProfileDto>.Success(userProfile);
            return result;
        }
    }
}
