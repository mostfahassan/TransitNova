namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Queries;

public interface IGetAdminTripsQueryService
{
    Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetAdminTripsAsync(UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default);
}