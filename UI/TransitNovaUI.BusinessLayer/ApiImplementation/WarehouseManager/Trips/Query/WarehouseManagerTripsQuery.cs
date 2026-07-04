using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Trips.Query
{
    public class WarehouseManagerTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IWarehouseManagerTripsQuery
    {
        public Task<ApiResponse<UiTripDetailsDto>> GetWarehouseManagerTripByIdAsync(Guid warehouseId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerTrips.GetTripByIdUrl, ("tripId", tripId), ("warehouseId", warehouseId)));

            return SendQueryRequestAsync<UiTripDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetWarehouseManagerTripsAsync(Guid warehouseId, UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerTrips.GetTripsUrl, ("warehouseId", warehouseId), ("TripType", filter.TripType), ("Status", filter.Status), ("CreatedAt", filter.CreatedAt), ("From", filter.From), ("To", filter.To), ("CreatedBy", filter.CreatedBy), ("CarrierId", filter.CarrierId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiTripDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }
    }
}
