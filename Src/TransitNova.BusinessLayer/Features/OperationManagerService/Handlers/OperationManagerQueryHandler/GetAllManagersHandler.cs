using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetAllManagersHandler(IOperationManagerQueryRepository operationManager, ILogger<GetActiveManagersHandler> logger) : IQueryHandler<GetAllManagersQuery, Result<IEnumerable<OperationManagerProfileDto>>>
    {
        public async Task<Result<IEnumerable<OperationManagerProfileDto>>> Handle(GetAllManagersQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving active operation managers");

            var allManager = await operationManager.GetAllAsync(cancellationToken);

            if (allManager.Count == 0)
            {
                logger.LogInformation("No active operation managers found");

                return Result <IEnumerable<OperationManagerProfileDto>>.Success([]);
            }
            logger.LogInformation("Successfully retrieved {Count} active operation managers", allManager.Count);

            return Result<IEnumerable<OperationManagerProfileDto>>.Success(allManager);
        }
    }
}
