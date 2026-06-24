
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository
{
    public interface ISystemLogCommands
    {
        Task LogAsync(SystemActivityLog log, CancellationToken cancellationToken);
    }
}
