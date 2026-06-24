namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

public interface ITrackShipmentQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/shipments/{trackingNumber:string}";

    Task<ApiResponse<UiRetrieveShipmentSummaryDto>> TrackShipmentAsync(string trackingNumber, CancellationToken cancellationToken = default);
}

