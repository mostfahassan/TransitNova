using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Vehicles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Vehicles.Query
{
    public class CarrierVehiclesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ICarrierVehiclesQuery
    {
        public Task<ApiResponse<UiVehicleDto?>> GetCarrierVehicleAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierVehicles.GetCarrierVehicleUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<UiVehicleDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}


