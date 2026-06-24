
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository
{
    public interface IOperationManagerQueryRepository
    {
        Task<Guid> GetUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Guid>> GetOperationManagersIdsAsync(CancellationToken cancellationToken);
        Task<string?> GetOperationManagerNameAsync(Guid userId, CancellationToken cancellationToken);
        Task<OperationManagerProfileDto?> GetOperationManagerProfileAsync(Guid id, CancellationToken cancellationToken);
        Task<List<OperationManagerProfileDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<OperationManagerProfileDto>> GetActiveAsync(CancellationToken cancellationToken);
        Task<PagedResult<CarrierSummaryDetailsDto>> GetHandledCarriersByOperationManagerAsync(Guid operationManagerId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<RetrieveShipmentSummaryDto>> GetHandledShipmentsByOperationManagerAsync(Guid operationManagerId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }

}
