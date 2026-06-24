
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
namespace TransitNova.BusinessLayer.Features.Admin.Handler
{
    public sealed class GetAdminDashboardHandler(IAdminDashboard adminDashboard , ILogger<GetAdminDashboardHandler> logger) : IQueryHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
    {
        public async Task<Result<AdminDashboardDto>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Building admin dashboard....");

            var dashboard = await adminDashboard.BuildAsync(cancellationToken);

            logger.LogInformation("Admin dashboard built successfully.");

            return Result<AdminDashboardDto>.Success(dashboard);
        }
    }
}
