namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IGetCarrierByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/carriers/{carrierId:guid}";

    Task<ApiResponse<UiCarrierProfileDto>> GetCarrierByIdAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

