using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.OperationManagerDashboard;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler
{
    public sealed class GetOperationManagerDashboardHandler(IOperationManagerDashboard dashboard)
        : IQueryHandler<GetOperationManagerDashboardQuery, Result<OperationManagerDashboardDto>>
    {
        public async Task<Result<OperationManagerDashboardDto>> Handle(GetOperationManagerDashboardQuery request, CancellationToken cancellationToken)
        {
         
            var dashboardData = await dashboard.BuildAsync(request.OperationManagerId, cancellationToken);
            return dashboardData;

        }
    }
}
