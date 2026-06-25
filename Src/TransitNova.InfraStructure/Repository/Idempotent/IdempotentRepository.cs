
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Idempotent
{
    internal class IdempotentRepository(AppDbContext context) : IIdempotentRepository
    {
        public async Task CreateRequestAsync(Guid idempotentKey, string instanceName, string response , CancellationToken cancellationToken)
        {
            var table = new IdempotentTable
            {
                RequestId = idempotentKey ,
                InstanceName = instanceName ,
                CreatedAt = DateTime.UtcNow,
                ResponseJson = response 
            };

            await context.AddAsync(table,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken)
          => await context.IdempotentTableKey.AnyAsync(r => r.RequestId == requestId,cancellationToken);

        public async Task<string?> ReturnRequestIfExistsAsync(Guid requestId, CancellationToken cancellationToken)

          => await context.IdempotentTableKey.AsNoTracking().Where(r => r.RequestId == requestId)
               .Select(r => r.ResponseJson)
               .FirstOrDefaultAsync(cancellationToken);

        public async Task RemoveRequestAsync(Guid requestId, CancellationToken cancellationToken)
        {
            var request = await context.IdempotentTableKey
                .Where(r => r.RequestId == requestId)
                .FirstOrDefaultAsync(cancellationToken);

            if (request is null)
                return;

            context.IdempotentTableKey.Remove(request);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}