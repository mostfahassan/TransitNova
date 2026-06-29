using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.WarehouseManagerDashboardService;
namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Handlers.ApplyQueries.WarehouseManagers
{
    public sealed class GetWarehouseManagerDashboardHandler(
        IWarehouseManagerDashboard managerDashboard,
        ILogger<GetWarehouseManagerDashboardHandler> logger) : IQueryHandler<GetWarehouseManagerDashboardQuery, Result<WarehouseManagerDashboardDto>>
    {
        public async Task<Result<WarehouseManagerDashboardDto>> Handle(GetWarehouseManagerDashboardQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Building warehouse manager dashboard....");

            var dashboard = await managerDashboard.BuildAsync(request.ManagerId, cancellationToken);

            if (dashboard is null)
            {
                logger.LogWarning("Warehouse manager dashboard not found. ManagerId: {ManagerId}", request.ManagerId);
                return Result<WarehouseManagerDashboardDto>.NotFound(Errors.NotFound("Warehouse manager dashboard not found."));
            }
            logger.LogInformation("Warehouse manager dashboard built successfully.");
            return Result<WarehouseManagerDashboardDto>.Success(dashboard);
        }
    }
}
