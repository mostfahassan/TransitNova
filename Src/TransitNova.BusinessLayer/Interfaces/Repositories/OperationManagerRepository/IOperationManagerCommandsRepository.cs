using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository
{
    public interface IOperationManagerCommandsRepository
    {
        Task UpdateAsync(OperationManagerProfile operationManager, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
