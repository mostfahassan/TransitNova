namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Vehicles.Queries;

public interface IGetCarrierVehicleQueryService
{
    Task<ApiResponse<UiVehicleDto?>> GetCarrierVehicleAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

