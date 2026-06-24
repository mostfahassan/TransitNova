using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyQueries
{
    public class GetShipmentStatisticsHandler(ILogger<GetShipmentStatisticsHandler> logger ,IShipmentQueryRepository shipment) : IQueryHandler<GetShipmentStatisticsQuery, Result<Dictionary<ShipmentStatuses, int>>>
    {
        public async Task<Result<Dictionary<ShipmentStatuses, int>>> Handle(GetShipmentStatisticsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Loading current shipment statistics grouped by latest shipment status");
            var shipmentStats = await shipment.GetShipmentCountInStatusAsync(cancellationToken);
            if (shipmentStats.Count == 0)
            {
                logger.LogWarning("No shipment statistics found");
                return Result<Dictionary<ShipmentStatuses, int>>.Success([]);

            }
            logger.LogInformation("Shipment statistics loaded successfully with {Count} grouped statuses", shipmentStats.Count);
            return Result<Dictionary<ShipmentStatuses, int>>.Success(shipmentStats);
        }

    }
} 

