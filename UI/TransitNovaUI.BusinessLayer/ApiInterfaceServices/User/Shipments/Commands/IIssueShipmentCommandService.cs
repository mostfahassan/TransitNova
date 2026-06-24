namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IIssueShipmentCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}/issue";

    Task<ApiResponse> IssueShipmentAsync(Guid shipmentId, UiIssueShipmentReason request, CancellationToken cancellationToken = default);
}

