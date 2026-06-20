using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository
{
    public interface IReceiverRepository
    {
        Task CreateReceiver(ReceiverProfile receiver, CancellationToken ct);
    }
}
