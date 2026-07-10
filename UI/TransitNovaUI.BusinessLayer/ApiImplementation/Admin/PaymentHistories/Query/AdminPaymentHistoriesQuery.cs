using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.PaymentHistories.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.PaymentHistories.Query;

internal sealed class AdminPaymentHistoriesQuery(IHttpHandler httpHandler, HttpClient httpClient)
    : ApiServiceBase(httpHandler, httpClient), IAdminPaymentHistoriesQuery
{
    public Task<ApiResponse<UiPagedResult<UiPaymentHistoryDetailsDto>>> GetAdminPaymentHistoriesAsync(
        UiPaymentHistoryFilterDto filter,
        string bearerToken,
        CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(
            ApiRoutes.AdminPaymentHistories.GetPaymentHistoriesUrl,
            ("PaymentId", filter.PaymentId),
            ("PaymentStatus", filter.PaymentStatus),
            ("PaymentMethod", filter.PaymentMethod),
            ("CreatedAt", filter.CreatedAt),
            ("CreatedAtFrom", filter.CreatedAtFrom),
            ("CreatedAtTo", filter.CreatedAtTo),
            ("CreatedBy", filter.CreatedBy),
            ("PageNumber", filter.PageNumber),
            ("PageSize", filter.PageSize)));

        return SendQueryRequestAsync<UiPagedResult<UiPaymentHistoryDetailsDto>>(
            HttpMethod.Get,
            url,
            bearerToken,
            cancellationToken);
    }
}
