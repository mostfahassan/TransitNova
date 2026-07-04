using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository
{
    public interface IIdempotentRepository
    {
        Task<IdempotentTable?> ReturnRequestIfExistsAsync(Guid requestId, CancellationToken cancellationToken);
        Task CreateRequestAsync(Guid idempotentKey ,string instanceName , string response, string hashedRequest, CancellationToken cancellationToken);
        Task RemoveRequestAsync(Guid requestId, CancellationToken cancellationToken);
    }
}