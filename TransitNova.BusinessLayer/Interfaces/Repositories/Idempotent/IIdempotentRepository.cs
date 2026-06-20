
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.Idempotent
{
    public interface IIdempotentRepository
    {
        Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken);
        Task CreateRequestAsync(Guid idempotentKey ,string instanceName,CancellationToken cancellationToken);
    }
}
