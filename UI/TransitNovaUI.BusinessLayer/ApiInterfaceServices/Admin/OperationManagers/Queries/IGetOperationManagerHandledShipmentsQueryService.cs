namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagerHandledShipmentsQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

