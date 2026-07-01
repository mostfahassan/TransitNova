namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

public interface IDeleteVehicleCommandService
{
    Task<ApiResponse> DeleteVehicleAsync(Guid vehicleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

