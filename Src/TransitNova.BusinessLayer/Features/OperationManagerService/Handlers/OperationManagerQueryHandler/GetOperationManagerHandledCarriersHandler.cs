using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetOperationManagerHandledCarriersHandler(IOperationManagerQueryRepository operationManagerRepo,
        ILogger<GetOperationManagerHandledCarriersHandler> logger)

      : IQueryHandler<GetOperationManagerHandledCarriersQuery, Result<PagedResult<CarrierSummaryDetailsDto>>>
    {
        public async Task<Result<PagedResult<CarrierSummaryDetailsDto>>> Handle(GetOperationManagerHandledCarriersQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving handled carriers for operation manager {OperationManagerId}. PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.OperationManagerId,
                request.PageNumber,
                request.PageSize);

            var handledCarriers = await operationManagerRepo.GetHandledCarriersByOperationManagerAsync(request.OperationManagerId, request.PageNumber, request.PageSize, cancellationToken);

            if (handledCarriers.TotalCount == 0)
            {
                logger.LogInformation("No handled carriers found for operation manager {OperationManagerId}", request.OperationManagerId);
                var emptyResult = Result<PagedResult<CarrierSummaryDetailsDto>>.Success(handledCarriers);
                return emptyResult;
            }
            logger.LogInformation("Successfully retrieved {TotalCount} handled carriers for operation manager {OperationManagerId}", handledCarriers.TotalCount, request.OperationManagerId);
            var result = Result<PagedResult<CarrierSummaryDetailsDto>>.Success(handledCarriers);
            return result;
        }
    }
}
