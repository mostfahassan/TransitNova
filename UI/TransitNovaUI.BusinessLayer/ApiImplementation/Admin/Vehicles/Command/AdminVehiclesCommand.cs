using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Command
{
    public class AdminVehiclesCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminVehiclesCommand
    {
        public Task<ApiResponse<UiVehicleDto>> CreateVehicleAsync(UiCreateVehicleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateVehicleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.CreateVehicleUrl));

            return SendRequestAsync<UiVehicleDto>(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteVehicleAsync(Guid vehicleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Vehicles.DeleteVehicleUrl, ("vehicleId", vehicleId)));


            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);
        }

    }
}
