using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Queries
{
    public class GetShipmentHistoriesHandler(IShipmentQueryRepository shipment,
        ILogger<GetShipmentHistoriesHandler> logger)
        : IQueryHandler<GetShipmentHistoriesQuery, Result<IEnumerable<RetrieveShipmentStatusDto>>>
    {
        public async Task<Result<IEnumerable<RetrieveShipmentStatusDto>>> Handle(
         GetShipmentHistoriesQuery request,
         CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving shipment history for ShipmentId {ShipmentId}", request.ShipmentId);
            var shipmentHistory = await shipment.GetShipmentHistoriesAsync(request.ShipmentId, cancellationToken);
            if (!shipmentHistory.Any())
            {
                logger.LogWarning("No shipment history found for ShipmentId {ShipmentId}", request.ShipmentId);


                var emptyResult = Result<IEnumerable<RetrieveShipmentStatusDto>>.Success([]);
                return emptyResult;
            }
            logger.LogInformation("Successfully retrieved {Count} shipment history records for ShipmentId {ShipmentId}", shipmentHistory.Count(),
                   request.ShipmentId);


            var result = Result<IEnumerable<RetrieveShipmentStatusDto>>.Success(shipmentHistory);
            return result;
        }
    }
}
