using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetOperationManagerDashboardHandler(
          IShipmentQueryRepository shipmentRepository,
          ILogger<GetOperationManagerDashboardHandler> logger 
        )
        : IQueryHandler<GetOperationManagerDashboardQuery, Result<OperationManagerDashboardDto>>
    {
        public async Task<Result<OperationManagerDashboardDto>> Handle(GetOperationManagerDashboardQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting operation manager dashboard loading");

            var shipments = await shipmentRepository.FilterAsync(new ShipmentFilterDto { PageNumber = 1, PageSize = 8 }, cancellationToken);

            var shipmentStats = await shipmentRepository.GetShipmentCountInStatusAsync(cancellationToken);
            logger.LogDebug("Dashboard queries completed successfully");

            logger.LogInformation("Retrieved {ShipmentCount} recent shipments",shipments.TotalCount);

            logger.LogInformation("Shipment statistics retrieved successfully: {@ShipmentStats}", shipmentStats);

            logger.LogInformation("Building operation manager dashboard DTO");
            
            var dashboard = OperationManagerDashboardHelper.Build(shipments.Data, shipmentStats);

            logger.LogInformation("Operation manager dashboard DTO built successfully");

            var result = Result<OperationManagerDashboardDto>.Success(dashboard);
            return result;
        }

    }
}
