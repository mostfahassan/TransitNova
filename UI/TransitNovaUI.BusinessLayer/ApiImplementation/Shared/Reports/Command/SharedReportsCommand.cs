using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.DTOs.Reports;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Reports.Command;

public sealed class SharedReportsCommand(IHttpHandler httpHandler, HttpClient httpClient)
    : ApiServiceBase(httpHandler, httpClient), ISharedReportsCommand
{
    public Task<ApiResponse> RequestBundleReportAsync(UiBundleReportContract contract, string bearerToken, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Reports.RequestBundleReportUrl));
        return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, contract, idempotencyKey);
    }

    public Task<ApiResponse> RequestDashboardReportAsync(UiDashboardReportContract contract, string bearerToken, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Reports.RequestDashboardReportUrl));
        return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, contract, idempotencyKey);
    }
}