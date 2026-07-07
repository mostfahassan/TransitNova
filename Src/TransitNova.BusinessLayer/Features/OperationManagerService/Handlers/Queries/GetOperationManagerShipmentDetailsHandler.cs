using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Queries
{
    public sealed class GetOperationManagerShipmentDetailsHandler(
        IShipmentQueryRepository shipmentRepository,
        IOperationManagerQueryRepository operationManagerRepository,
        ILogger<GetOperationManagerShipmentDetailsHandler> logger)
        : IQueryHandler<GetOperationManagerShipmentDetailsQuery, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(GetOperationManagerShipmentDetailsQuery request, CancellationToken cancellationToken)
        {
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(request.OperationManagerId, cancellationToken);
            if (operationManagerId == Guid.Empty)
            {
                logger.LogWarning("Operation manager AppUser {OperationManagerAppUserId} does not have a profile record.", request.OperationManagerId);
                return Result<RetrieveShipmentDto>.Forbidden(Errors.Forbidden("Operation manager profile was not found."));
            }

            logger.LogDebug("Fetching shipment {ShipmentId} for operation manager app-user {OperationManagerAppUserId}.", request.ShipmentId, request.OperationManagerId);
            var shipment = await shipmentRepository.GetShipmentAsync(sh => sh.Id == request.ShipmentId, cancellationToken);
            if (shipment is null)
            {
                logger.LogInformation("Shipment {ShipmentId} was not found for operation manager app-user {OperationManagerAppUserId}.", request.ShipmentId, request.OperationManagerId);
                return Result<RetrieveShipmentDto>.NotFound(Errors.ShipmentNotFound("Shipment not found."));
            }

            return Result<RetrieveShipmentDto>.Success(shipment);
        }
    }
}