using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.OperationManager
{
    internal class OperationManagerRulesRepository(AppDbContext context) : IOperationManagerRulesRepository
    {
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
          => await context.OperationManagerProfiles.AnyAsync(op => op.AppUserId == id,cancellationToken);
    }
}
