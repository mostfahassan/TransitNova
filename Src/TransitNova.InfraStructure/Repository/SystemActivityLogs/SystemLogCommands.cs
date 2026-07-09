
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.SystemActivityLogs
{
    internal class SystemLogCommands(AppDbContext context) : ISystemLogCommands
    {
        public async Task LogAsync(SystemActivityLog log, CancellationToken cancellationToken)
        {
            await context.SystemActivityLogs.AddAsync(log,cancellationToken);
        }
    }
}
