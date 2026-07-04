using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.OperationManagerDashboard;
namespace TransitNova.BusinessLayer.Services.OperationManagerDashboardService
{
    internal class OperationManagerDashboard(IOperationManagerRulesRepository operationManager,
        IOperationManagerDashboardRepository dashboardQuery,
        ILogger<OperationManagerDashboard> logger) : IOperationManagerDashboard
    {
        public async Task<Result<OperationManagerDashboardDto>> BuildAsync(Guid operationManagerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting operation manager dashboard loading");

            var exists = await operationManager.ExistsAsync(operationManagerId, cancellationToken);
            if (!exists)
            {
                logger.LogWarning("Operation manager with ID {OperationManagerId} not found", operationManagerId);
                return Result<OperationManagerDashboardDto>.Failure(Errors.NotFound("Operation manager not found."));
            }

            var TotalHandledShipmentTask = dashboardQuery.TotalHandledShipmentsAsync(operationManagerId, cancellationToken);
            var TotalHandledCarriersTask = dashboardQuery.TotalHandledCarriersAsync(operationManagerId, cancellationToken);
            var ShipmentStatisticsTask = dashboardQuery.GetShipmentCountInStatusAsync(cancellationToken);
            var shipmentsTask = dashboardQuery.GetShipmentsAsync(cancellationToken);

            await Task.WhenAll(TotalHandledShipmentTask, TotalHandledCarriersTask, ShipmentStatisticsTask, shipmentsTask);

            var totalHandledShipments = TotalHandledShipmentTask.Result;
            var totalHandledCarriers = TotalHandledCarriersTask.Result;
            var shipmentStatistics = ShipmentStatisticsTask.Result;
            var shipments = shipmentsTask.Result;


            var dashboardDto = OperationManagerDashboardHelper.Build(shipments.Data, shipmentStatistics, totalHandledShipments, totalHandledCarriers);

            return Result<OperationManagerDashboardDto>.Success(dashboardDto);
        }
    }
}
