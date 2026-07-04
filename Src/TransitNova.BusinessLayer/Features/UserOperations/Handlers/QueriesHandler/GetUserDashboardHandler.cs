using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public class GetUserDashboardHandler(
        IUserQueryRepository repo,
        ILogger<GetUserDashboardHandler> logger)
        : IQueryHandler<GetUserDashboardQuery, Result<ProfileDashboardDto>>
    {
        async Task<Result<ProfileDashboardDto>> IRequestHandler<GetUserDashboardQuery, Result<ProfileDashboardDto>>.Handle(GetUserDashboardQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting retrieving shipments for User {UserId}", request.AppUserId);
            var userShipments = await repo.GetUserShipmentsAsync(request.AppUserId, cancellationToken);

            if (!userShipments.Any())
            {
                logger.LogWarning("No shipments found for User {UserId}", request.AppUserId);

                var emptyResult = Result<ProfileDashboardDto>.Success(ProfileDashboardHelper.Empty());
                return emptyResult;
            }

            var statusCounts = await repo.GetShipmentCountInStatusAsync(request.AppUserId, cancellationToken);

            var dashboard = ProfileDashboardHelper.Build(userShipments, statusCounts);

            logger.LogInformation("Retrieved {Count} shipments for User {UserId}", userShipments.Count(), request.AppUserId);

            var result = Result<ProfileDashboardDto>.Success(dashboard);
            return result;
        }
    }
}
