namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;

public interface IAddCarrierAdditionalInfoCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/carriers/additional-info";

    Task<ApiResponse> AddCarrierAdditionalInfoAsync(UiAdditionalInfoDto request, CancellationToken cancellationToken = default);
}

