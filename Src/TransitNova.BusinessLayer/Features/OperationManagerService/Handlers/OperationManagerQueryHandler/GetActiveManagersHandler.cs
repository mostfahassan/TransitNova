
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetActiveManagersHandler(IOperationManagerQueryRepository operationManager, ILogger<GetActiveManagersHandler>logger) : IQueryHandler<GetActiveManagerQuery, Result<List<OperationManagerProfileDto>>>
    {
        public async Task<Result<List<OperationManagerProfileDto>>> Handle(GetActiveManagerQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving active operation managers");

            var activeManagers = await operationManager.GetActiveAsync(cancellationToken);

            if (activeManagers.Count == 0)
            {
                logger.LogInformation("No active operation managers found");

                return Result<List<OperationManagerProfileDto>>.Success([]);
            }
            logger.LogInformation("Successfully retrieved {Count} active operation managers", activeManagers.Count);

            return Result<List<OperationManagerProfileDto>>.Success(activeManagers);
        }



    }

}
