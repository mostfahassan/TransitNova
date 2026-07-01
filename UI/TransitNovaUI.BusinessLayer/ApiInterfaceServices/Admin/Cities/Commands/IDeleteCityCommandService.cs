namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface IDeleteCityCommandService
{
    Task<ApiResponse> DeleteCityAsync(int cityId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

