namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehiclesQueryService
{
    Task<ApiResponse<List<UiVehicleDto>>> GetVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

