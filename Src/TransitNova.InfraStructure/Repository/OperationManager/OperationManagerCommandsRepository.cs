

using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.OperationManager
{
    internal class OperationManagerCommandsRepository(AppDbContext context) : IOperationManagerCommandsRepository
    {
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {   var op = await context.OperationManagerProfiles.FindAsync(id, cancellationToken);
            if (op is null)
            {
                return false;
            }
            context.OperationManagerProfiles.Remove(op);
            return true;
        }

        public async Task UpdateAsync(OperationManagerProfile operationManager, CancellationToken cancellationToken)
        {
            context.OperationManagerProfiles.Update(operationManager);
        }
    }
}
