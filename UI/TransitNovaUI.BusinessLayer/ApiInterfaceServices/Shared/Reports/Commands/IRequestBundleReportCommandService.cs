using TransitNovaUI.BusinessLayer.DTOs.Reports;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Commands;

public interface IRequestBundleReportCommandService
{
    Task<ApiResponse> RequestBundleReportAsync(UiBundleReportContract contract, string bearerToken, string idempotencyKey, CancellationToken cancellationToken = default);
}