using TransitNovaUI.BusinessLayer.DTOs.Reports;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Commands;

public interface IRequestDashboardReportCommandService
{
    Task<ApiResponse> RequestDashboardReportAsync(UiDashboardReportContract contract, string bearerToken, string idempotencyKey, CancellationToken cancellationToken = default);
}