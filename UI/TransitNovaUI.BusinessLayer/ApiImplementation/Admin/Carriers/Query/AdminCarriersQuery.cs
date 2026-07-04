using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Carriers.Query
{
    public class AdminCarriersQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminCarriersQuery
    {
        public Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetAdminCarriersAsync(UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.GetAdminCarriersUrl, ("Status", filter.Status), ("MinRating", filter.MinRating), ("MaxRating", filter.MaxRating), ("MinYearsOfExperience", filter.MinYearsOfExperience), ("MaxYearsOfExperience", filter.MaxYearsOfExperience), ("City", filter.City), ("CityId", filter.CityId), ("SearchTerm", filter.SearchTerm), ("AvailableFrom", filter.AvailableFrom), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("VehicleCapacityWeight", filter.VehicleCapacityWeight), ("VehicleType", filter.VehicleType), ("ServedZones", filter.ServedZones)));

            return SendQueryRequestAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCarrierProfileDto>> GetAdminCarrierByIdAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.GetAdminCarrierByIdUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<UiCarrierProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiRetrieveShipmentDto>> GetAdminCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.GetAdminCarrierShipmentByIdUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCarrierShipmentListDto>> GetAdminCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.GetAdminCarrierShipmentsUrl, ("carrierId", carrierId), ("Status", filter.Status), ("Mode", filter.Mode), ("SearchTerm", filter.SearchTerm), ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiCarrierShipmentListDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}

