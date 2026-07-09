using TransitNova.Domain.Enums.Reports;

namespace TransitNova.InfraStructure.Reports.Interface
{
    public interface IReportGenerator
    {
        ReportType ReportType { get; }
        Task<string> GenerateReportAsync(Guid reportId, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
