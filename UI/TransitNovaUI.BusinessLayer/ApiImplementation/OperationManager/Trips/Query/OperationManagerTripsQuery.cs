using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Trips.Query
{
    public class OperationManagerTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IOperationManagerTripsQuery
    {
        public Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetOperationManagerTripsAsync(UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.GetTripsUrl,
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

        public Task<ApiResponse<UiTripDetailsDto>> GetOperationManagerTripByIdAsync(Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.GetTripByIdUrl, ("tripId", tripId)));
            return SendQueryRequestAsync<UiTripDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}