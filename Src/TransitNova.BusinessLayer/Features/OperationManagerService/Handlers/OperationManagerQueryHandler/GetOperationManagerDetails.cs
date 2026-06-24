
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetOperationManagerDetails(IOperationManagerQueryRepository operationManagerRepo, ILogger<GetOperationManagerHandledShipmentsQuery> logger) : IQueryHandler<GetOperationManagerDetailsQuery, Result<OperationManagerProfileDto>>
    {
        public async Task<Result<OperationManagerProfileDto>> Handle(GetOperationManagerDetailsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving operation manager profile with ID: {OperationManagerId}", request.OperationManagerId);

            var operationManager = await operationManagerRepo.GetOperationManagerProfileAsync(request.OperationManagerId, cancellationToken);
            if (operationManager is null)
            {
                logger.LogWarning("Operation manager profile with ID: {OperationManagerId} was not found", request.OperationManagerId);

                var notFoundResult = Result<OperationManagerProfileDto>.NotFound(
                    Errors.NotFound($"Operation manager with ID '{request.OperationManagerId}' was not found."));
                return notFoundResult;
            }

            logger.LogInformation("Successfully retrieved operation manager profile with ID: {OperationManagerId}", request.OperationManagerId);
            var result = Result<OperationManagerProfileDto>.Success(operationManager);
            return result;
        }
    }
}
