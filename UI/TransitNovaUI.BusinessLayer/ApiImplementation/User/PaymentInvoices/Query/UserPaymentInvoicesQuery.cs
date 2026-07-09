using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.PaymentInvoices.Query;

public sealed class UserPaymentInvoicesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserPaymentInvoicesQuery
{
    public Task<ApiResponse<UiInvoiceDto>> GetUserPaymentInvoiceAsync(Guid paymentId, string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserPaymentInvoices.GetUserPaymentInvoiceUrl, ("paymentId", paymentId)));

        return SendQueryRequestAsync<UiInvoiceDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }

    public Task<ApiResponse<IEnumerable<UiInvoiceDto>>> GetUserPaymentInvoicesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.UserPaymentInvoices.GetUserPaymentInvoicesUrl);

        return SendQueryRequestAsync<IEnumerable<UiInvoiceDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }
}
