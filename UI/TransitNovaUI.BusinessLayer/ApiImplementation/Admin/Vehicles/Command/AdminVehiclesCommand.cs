using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Command
{
    public class AdminVehiclesCommand(IHttpHandler httpHandler, HttpClient httpClient) : IAdminVehiclesCommand
    {
        public async Task<ApiResponse<UiVehicleDto>> CreateVehicleAsync(UiCreateVehicleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateVehicleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.CreateVehicleUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiVehicleDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteVehicleAsync(Guid vehicleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.DeleteVehicleUrl, ("vehicleId", vehicleId)));


            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
