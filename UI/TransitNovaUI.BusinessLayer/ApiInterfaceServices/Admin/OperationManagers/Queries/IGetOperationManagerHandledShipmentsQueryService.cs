namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagerHandledShipmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/operation-managers/{operationManagerId:guid}/handled-shipments";

    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

