using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Reports
{
    internal class ReportRequestCommands(AppDbContext context) : IReportRequestCommands
    {
        public async Task AddReportRequstAsync(ReportRequest request, CancellationToken cancellationToken)
         =>  await context.ReportRequests.AddAsync(request, cancellationToken);
    }
}
