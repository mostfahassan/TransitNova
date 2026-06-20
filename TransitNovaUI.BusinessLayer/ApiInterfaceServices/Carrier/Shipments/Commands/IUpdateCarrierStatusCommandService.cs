namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface IUpdateCarrierStatusCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/status";

    Task<ApiResponse> UpdateCarrierStatusAsync(Guid carrierId, UiChangeCarrierStatusDto request, CancellationToken cancellationToken = default);
}

