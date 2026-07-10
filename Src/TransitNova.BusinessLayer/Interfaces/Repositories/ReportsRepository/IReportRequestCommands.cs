using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository
{
    public interface IReportRequestCommands
    {
        Task AddReportRequestAsync(ReportRequest request, CancellationToken cancellationToken);
        Task ExpireReportsBulkAsync(IEnumerable<Guid> reportIds, CancellationToken cancellationToken);
    }
}
