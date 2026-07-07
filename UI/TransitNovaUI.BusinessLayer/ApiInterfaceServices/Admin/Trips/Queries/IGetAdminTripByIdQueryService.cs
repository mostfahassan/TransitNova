namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Queries;

public interface IGetAdminTripByIdQueryService
{
    Task<ApiResponse<UiTripDetailsDto>> GetAdminTripByIdAsync(Guid tripId, string bearerToken, CancellationToken cancellationToken = default);
}