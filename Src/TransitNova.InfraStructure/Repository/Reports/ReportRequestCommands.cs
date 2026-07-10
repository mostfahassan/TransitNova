using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Reports;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Reports
{
    internal class ReportRequestCommands(AppDbContext context) : IReportRequestCommands
    {
        public async Task AddReportRequestAsync(ReportRequest request, CancellationToken cancellationToken)
         =>  await context.ReportRequests.AddAsync(request, cancellationToken);

        public async Task ExpireReportsBulkAsync(IEnumerable<Guid> reportIds, CancellationToken cancellationToken)
          => await context.ReportRequests
                .Where(report => reportIds.Contains(report.Id))
                 .ExecuteUpdateAsync(setters => setters
                 .SetProperty(x => x.FilePath, x => null)
                 .SetProperty(x => x.ReportStatus, x => ReportStatus.Deleted)
                 .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
                 .SetProperty(x => x.FileSize, x => 0), cancellationToken);
    }
}
