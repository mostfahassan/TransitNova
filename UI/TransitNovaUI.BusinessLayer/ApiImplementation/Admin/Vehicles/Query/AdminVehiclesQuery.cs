using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Query
{
    public class AdminVehiclesQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminVehiclesQuery

    {
        public async Task<ApiResponse<List<UiVehicleDto>>> GetActiveVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetActiveVehiclesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiVehicleDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiVehicleDto?>> GetVehicleByIdAsync(Guid vehicleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehicleByIdUrl, ("vehicleId", vehicleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get,  url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiVehicleDto?>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiVehicleDto?>> GetVehicleByPlateNumberAsync(string plateNumber, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehicleByPlateNumberUrl, ("plateNumber", plateNumber)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiVehicleDto?>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<List<UiVehicleDto>>> GetVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.GetVehiclesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiVehicleDto>>(httpResponse, cancellationToken);
        }

    }
}
