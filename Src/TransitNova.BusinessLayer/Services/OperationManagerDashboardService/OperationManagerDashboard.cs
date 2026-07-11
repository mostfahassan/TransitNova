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

            var handledSummaryTask = dashboardQuery.GetHandledSummaryAsync(operationManagerId, cancellationToken);
            var shipmentStatisticsTask = dashboardQuery.GetShipmentCountInStatusAsync(cancellationToken);
            var shipmentsTask = dashboardQuery.GetShipmentsAsync(cancellationToken);

            await Task.WhenAll(handledSummaryTask, shipmentStatisticsTask, shipmentsTask);

            var handledSummary = handledSummaryTask.Result;
            var shipmentStatistics = shipmentStatisticsTask.Result;
            var shipments = shipmentsTask.Result;

            var dashboardDto = OperationManagerDashboardHelper.Build(shipments.Data, shipmentStatistics, handledSummary.TotalHandledShipments, handledSummary.TotalHandledCarriers);

            return Result<OperationManagerDashboardDto>.Success(dashboardDto);
        }
    }
}
