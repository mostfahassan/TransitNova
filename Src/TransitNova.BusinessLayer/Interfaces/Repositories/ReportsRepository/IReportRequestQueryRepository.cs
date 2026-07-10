using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository
{
    public interface IReportRequestQueryRepository
    {
        Task<ReportDownloadDto?> GetReportDownloadAsync(Guid reportId, Guid requestedBy, bool canAccessAll, CancellationToken cancellationToken);
        Task<IReadOnlyList<ReportClanableData>> ReportCleanableData(CancellationToken cancellationToken);
    }
}
