namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetActiveVehiclesQueryService
{
    Task<ApiResponse<List<UiVehicleDto>>> GetActiveVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

