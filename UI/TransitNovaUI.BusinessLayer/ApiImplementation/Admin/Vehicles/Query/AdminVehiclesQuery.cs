using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Query
{
    public class AdminVehiclesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminVehiclesQuery

    {
        public Task<ApiResponse<List<UiVehicleDto>>> GetActiveVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetActiveVehiclesUrl));

            return SendQueryRequestAsync<List<UiVehicleDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiVehicleDto?>> GetVehicleByIdAsync(Guid vehicleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehicleByIdUrl, ("vehicleId", vehicleId)));

            return SendQueryRequestAsync<UiVehicleDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiVehicleDto?>> GetVehicleByPlateNumberAsync(string plateNumber, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehicleByPlateNumberUrl, ("plateNumber", plateNumber)));

            return SendQueryRequestAsync<UiVehicleDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<List<UiVehicleDto>>> GetVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehiclesUrl));

            return SendQueryRequestAsync<List<UiVehicleDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
