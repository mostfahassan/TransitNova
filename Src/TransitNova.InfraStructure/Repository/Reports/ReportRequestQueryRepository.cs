using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.Domain.Enums.Reports;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Reports
{
    internal sealed class ReportRequestQueryRepository(AppDbContext context) : IReportRequestQueryRepository
    {
        public async Task<IReadOnlyList<ReportClanableData>> ReportCleanableData(CancellationToken cancellationToken)
            => await context.ReportRequests
                .AsNoTracking()
                .Where(report => report.ReportStatus == ReportStatus.Completed && report.FilePath != null && report.CompletedAt < DateTime.UtcNow.AddDays(-5))
                .Select(report => new ReportClanableData(
                    report.Id,
                    report.FilePath!))
                .ToListAsync(cancellationToken);

        public async Task<ReportDownloadDto?> GetReportDownloadAsync(Guid reportId, Guid requestedBy, bool canAccessAll, CancellationToken cancellationToken)
        {
            var query = context.ReportRequests
                .AsNoTracking()
                .Where(report => report.Id == reportId);

            if (!canAccessAll)
            {
                query = query.Where(report => report.RequestedBy == requestedBy);
            }

            return await query
                .Where(report => report.ReportStatus == ReportStatus.Completed && report.FilePath != null)
                .Select(report => new ReportDownloadDto(
                    report.FilePath!,
                    Path.GetFileName(report.FilePath!)))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
