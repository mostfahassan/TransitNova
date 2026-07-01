namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

public interface IGetHandledShipmentsQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

