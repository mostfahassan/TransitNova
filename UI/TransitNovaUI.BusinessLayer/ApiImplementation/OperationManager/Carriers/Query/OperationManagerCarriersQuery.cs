using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Query
{
    public class OperationManagerCarriersQuery(IHttpHandler httpHandler, HttpClient httpClient) :
        ApiServiceBase(httpHandler, httpClient), IOperationManagerCarriersQuery
    {
        public Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> FilterCarriersAsync(UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.FilterCarriersUrl, ("Status", filter.Status), ("MinRating", filter.MinRating), ("MaxRating", filter.MaxRating), ("MinYearsOfExperience", filter.MinYearsOfExperience), ("MaxYearsOfExperience", filter.MaxYearsOfExperience), ("City", filter.City), ("CityId", filter.CityId), ("SearchTerm", filter.SearchTerm), ("AvailableFrom", filter.AvailableFrom), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("VehicleCapacityWeight", filter.VehicleCapacityWeight), ("VehicleType", filter.VehicleType), ("ServedZones", filter.ServedZones)));

            return SendQueryRequestAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCarrierProfileDto>> GetCarrierByIdAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierByIdUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<UiCarrierProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierShipmentByIdUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.GetCarrierShipmentsUrl, ("carrierId", carrierId), ("Status", filter.Status), ("Mode", filter.Mode), ("SearchTerm", filter.SearchTerm), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiCarrierShipmentListDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}

