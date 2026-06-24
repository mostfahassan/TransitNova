
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyQueries
{
    public class GetShipmentByIdHandler(ILogger<GetShipmentByIdHandler> logger
        , IShipmentQueryRepository shipmentRepo)
        : IQueryHandler<GetShipmentByIdQuery, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Fetching detailed shipment for Id: {ShipmentId}", request.ShipmentId);
            var detailedShipment = await shipmentRepo.CreateShipmentForUserAsync(request.ShipmentId, cancellationToken);

            if (detailedShipment is not null)
            {
                logger.LogInformation("Shipment {ShipmentId} retrieved successfully", request.ShipmentId);
                return Result<RetrieveShipmentDto>.Success(detailedShipment);
            }
            logger.LogInformation("Shipment with TrackingNumber {ShipmentId} not found", request.ShipmentId);
            return Result<RetrieveShipmentDto>.NotFound(Errors.ShipmentNotFound("Shipment Not Found"));
        }
    }
}
