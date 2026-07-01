namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IIssueShipmentCommandService
{
    Task<ApiResponse> IssueShipmentAsync(Guid shipmentId, UiIssueShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

