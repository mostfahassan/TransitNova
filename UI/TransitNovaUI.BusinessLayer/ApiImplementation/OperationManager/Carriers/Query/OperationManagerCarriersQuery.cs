using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Query
{
    public class OperationManagerCarriersQuery(IHttpHandler httpHandler, HttpClient httpClient) :
        IOperationManagerCarriersQueries
    {
        public async Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> FilterCarriersAsync(UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.FilterCarriersUrl, ("Status", filter.Status), ("MinRating", filter.MinRating), ("MaxRating", filter.MaxRating), ("MinYearsOfExperience", filter.MinYearsOfExperience), ("MaxYearsOfExperience", filter.MaxYearsOfExperience), ("City", filter.City), ("CityId", filter.CityId), ("SearchTerm", filter.SearchTerm), ("AvailableFrom", filter.AvailableFrom), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("VehicleCapacityWeight", filter.VehicleCapacityWeight), ("VehicleType", filter.VehicleType), ("ServedZones", filter.ServedZones)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiCarrierProfileDto>> GetCarrierByIdAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierByIdUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierProfileDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierShipmentByIdUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierShipmentsUrl, ("carrierId", carrierId), ("Status", filter.Status), ("Mode", filter.Mode), ("SearchTerm", filter.SearchTerm), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierShipmentListDto>(httpResponse, cancellationToken);
        }

    }
}
