using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetOperationManagerHandledShipmentsHandler(IOperationManagerQueryRepository operationManagerRepo,
        ILogger<GetOperationManagerHandledShipmentsQuery> logger) : IQueryHandler<GetOperationManagerHandledShipmentsQuery, Result<PagedResult<RetrieveShipmentSummaryDto>>>
    {
        public async Task<Result<PagedResult<RetrieveShipmentSummaryDto>>> Handle(GetOperationManagerHandledShipmentsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving handled shipments for operation manager {OperationManagerId}. PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.OperationManagerId,
                    request.PageNumber,
                    request.PageSize);

            var handledShipments = await operationManagerRepo.RetrieveHandledShipmentsByOperationManager(request.OperationManagerId, request.PageNumber, request.PageSize, cancellationToken);

            if (handledShipments.TotalCount == 0)
            {
                logger.LogInformation("No handled shipments found for operation manager {OperationManagerId}", request.OperationManagerId);

                var emptyResult = Result<PagedResult<RetrieveShipmentSummaryDto>>.Success(handledShipments);
                return emptyResult;
            }

            logger.LogInformation("Successfully retrieved {TotalCount} handled shipments for operation manager {OperationManagerId}", handledShipments.TotalCount, request.OperationManagerId);

            var result = Result<PagedResult<RetrieveShipmentSummaryDto>>.Success(handledShipments);
            return result;
        }
    }
}
