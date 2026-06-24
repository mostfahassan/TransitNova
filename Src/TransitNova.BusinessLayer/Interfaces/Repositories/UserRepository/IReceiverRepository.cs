using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository
{
    public interface IReceiverRepository
    {
        Task CreateReceiverAsync(ReceiverProfile receiver, CancellationToken ct);
    }
}
