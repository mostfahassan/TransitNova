
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public class TrackShipmentQueryHandler(ILogger<TrackShipmentQueryHandler> logger,
         IShipmentQueryRepository shipmentRepo)
        : IQueryHandler<TrackShipmentQuery, Result<RetrieveShipmentSummaryDto>>
    {
        public async Task<Result<RetrieveShipmentSummaryDto>>Handle(TrackShipmentQuery request, CancellationToken cancellationToken)
        {
            var detailedShipment = await shipmentRepo.GetByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

            if (detailedShipment is not null)
            {
                var result = Result<RetrieveShipmentSummaryDto>.Success(detailedShipment);
                return result;
            }

            logger.LogInformation("Shipment with TrackingNumber {TrackingNumber} not found", request.TrackingNumber);
            var notFoundResult = Result<RetrieveShipmentSummaryDto>.NotFound(Errors.NotFound("Shipment Not Found"));
            return notFoundResult;

        }
    }
}
