using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.User
{
    public class ReceiverRepository(AppDbContext context , ILogger<ReceiverRepository> logger) : IReceiverRepository
    {
        public async Task CreateReceiverAsync(ReceiverProfile receiver, CancellationToken ct)
        {
            logger.LogDebug("Start Creating Receiver profile");
            await context.ReceiverProfiles.AddAsync(receiver, ct);
          
        }
    }
}
