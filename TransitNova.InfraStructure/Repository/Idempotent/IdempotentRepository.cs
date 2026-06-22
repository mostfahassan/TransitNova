
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Idempotent
{
    internal class IdetmpoentRepository(AppDbContext context) : IIdempotentRepository
    {
        public async Task CreateRequestAsync(Guid idempotentKey, string instanceName, CancellationToken cancellationToken)
        {
            var table = new IdempotentTable
            {
                RequestId = idempotentKey ,
                InstanceName = instanceName ,
                CreatedAt = DateTime.UtcNow,
            };

            await context.AddAsync(table,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken)
          => await context.IdempotentTableKey.AnyAsync(r => r.RequestId == requestId,cancellationToken);
    }
}
