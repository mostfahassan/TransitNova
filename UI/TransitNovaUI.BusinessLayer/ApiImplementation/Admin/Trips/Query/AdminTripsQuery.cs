using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Trips.Query
{
    public class AdminTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminTripsQuery
    {
        public Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetAdminTripsAsync(UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminTrips.GetTripsUrl,
                ("Id", filter.Id),
                ("TripType", filter.TripType),
                ("Status", filter.Status),
                ("CreatedAt", filter.CreatedAt),
                ("From", filter.From),
                ("To", filter.To),
                ("CreatedBy", filter.CreatedBy),
                ("CarrierId", filter.CarrierId),
                ("WarehouseId", filter.WarehouseId),
                ("PageNumber", filter.PageNumber),
                ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiTripDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiTripDetailsDto>> GetAdminTripByIdAsync(Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminTrips.GetTripByIdUrl, ("tripId", tripId)));
            return SendQueryRequestAsync<UiTripDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}