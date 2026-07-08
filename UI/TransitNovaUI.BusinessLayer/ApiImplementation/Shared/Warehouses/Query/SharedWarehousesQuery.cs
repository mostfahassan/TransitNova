using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Warehouses.Queries;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Warehouses.Query;

public sealed class SharedWarehousesQuery(IHttpHandler httpHandler, HttpClient httpClient)
    : ApiServiceBase(httpHandler, httpClient), IGetSharedWarehousesQueryService
{
    public Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.SharedWarehouses.GetWarehousesUrl));
        return SendQueryRequestAsync<List<UiWarehouseDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }
}