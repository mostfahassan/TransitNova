namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

public interface ITrackShipmentQueryService
{
    Task<ApiResponse<UiRetrieveShipmentSummaryDto>> TrackShipmentAsync(string trackingNumber, string bearerToken, CancellationToken cancellationToken = default);
}

