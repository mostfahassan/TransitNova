using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Shipments.Query
{
    public class OperationManagerShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) 
        : IOperationManagerShipmentsQuery
    {
        public async Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> FilterShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.FilterShipmentsUrl, ("Status", filter.Status), ("Mode", filter.Mode), ("From", filter.From), ("To", filter.To), ("SenderId", filter.SenderId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiRetrieveShipmentDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetAssignedShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.GetAssignedShipmentsUrl, ("Status", filter.Status), ("Mode", filter.Mode), ("From", filter.From), ("To", filter.To), ("SenderId", filter.SenderId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiRetrieveShipmentDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRetrieveShipmentDto>> GetShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.GetShipmentByIdUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UiRetrieveShipmentStatusDto>>> GetShipmentHistoriesAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.GetShipmentHistoriesUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IEnumerable<UiRetrieveShipmentStatusDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetShipmentReviewQueueAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.GetShipmentReviewQueueUrl, ("Status", filter.Status), ("Mode", filter.Mode), ("From", filter.From), ("To", filter.To), ("SenderId", filter.SenderId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiRetrieveShipmentDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRetrieveShipmentDto>> ReviewShipmentAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.ReviewShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);
            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

    }
}
