using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository
{
    public interface IReportRequestCommands
    {
        Task AddReportRequstAsync(ReportRequest request, CancellationToken cancellationToken);
    }
}
