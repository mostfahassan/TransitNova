
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Queries
{
    public sealed class GetAssignedShipmentsHandler(ILogger<GetAssignedShipmentsHandler> logger ,IShipmentQueryRepository shipmentRepository)
        : IQueryHandler<GetAssignedShipmentsQuery, Result<PagedResult<RetrieveShipmentDto>>>
    {
        public async Task<Result<PagedResult<RetrieveShipmentDto>>> Handle(GetAssignedShipmentsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Loading operation manager assigned shipment queue");

            var statuses = new[]
            {
                ShipmentStatuses.AssignedToPickUpCarrier,
                ShipmentStatuses.AssignedToDeliveryCarrier,
                ShipmentStatuses.OutForPickup,
                ShipmentStatuses.OutForDelivery,
                ShipmentStatuses.InTransit
            };

            request.Dto.Status = statuses;

            logger.LogDebug("Assigned shipment statuses applied successfully: {@Statuses}", statuses);

            logger.LogDebug("Starting assigned shipment filtering with PageNumber={PageNumber}, PageSize={PageSize}", request.Dto.PageNumber, request.Dto.PageSize);

            var filteredShipment = await shipmentRepository.FilterAsync(request.Dto, cancellationToken);

            logger.LogInformation("Retrieved {ShipmentCount} assigned shipments",filteredShipment.TotalCount);

        
            if (!filteredShipment.Data.Any())
            {
                logger.LogWarning("No assigned shipments found matching the provided filtering criteria");

                return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipment);
            }

            logger.LogInformation("Assigned shipment queue loaded successfully");

            return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipment);
        }
    }
}
