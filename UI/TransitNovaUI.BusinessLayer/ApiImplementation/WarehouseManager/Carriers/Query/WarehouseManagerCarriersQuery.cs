using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Carriers.Query
{
    public class WarehouseManagerCarriersQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IWarehouseManagerCarriersQuery
    {
        public Task<ApiResponse<UiCarrierProfileDto>> GetWarehouseManagerCarrierByIdAsync(Guid warehouseId, Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerCarriers.GetCarrierByIdUrl, ("carrierId", carrierId), ("warehouseId", warehouseId)));

            return SendQueryRequestAsync<UiCarrierProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetWarehouseManagerCarriersAsync(Guid warehouseId, UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerCarriers.GetCarriersUrl, ("warehouseId", warehouseId), ("Status", filter.Status), ("MinRating", filter.MinRating), ("MaxRating", filter.MaxRating), ("MinYearsOfExperience", filter.MinYearsOfExperience), ("MaxYearsOfExperience", filter.MaxYearsOfExperience), ("City", filter.City), ("CityId", filter.CityId), ("SearchTerm", filter.SearchTerm), ("AvailableFrom", filter.AvailableFrom), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("VehicleCapacityWeight", filter.VehicleCapacityWeight), ("VehicleType", filter.VehicleType), ("ServedZones", filter.ServedZones)));

            return SendQueryRequestAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}
