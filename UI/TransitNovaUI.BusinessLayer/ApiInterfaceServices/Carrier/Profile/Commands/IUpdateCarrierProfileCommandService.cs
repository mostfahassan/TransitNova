namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;

public interface IUpdateCarrierProfileCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/carriers/profile";

    Task<ApiResponse<UiCarrierProfileDto>> UpdateCarrierProfileAsync(UiUpdateCarrierDto request, CancellationToken cancellationToken = default);
}

