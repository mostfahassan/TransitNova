using TransitNova.Domain.Enums.Reports;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Reports.Reports
{
    internal class ShipmentAnalyticsReportGenerator : IReportGenerator
    {
        public ReportType ReportType => ReportType.ShipmentAnalysisReport;

        public Task<string> GenerateReportAsync(Guid reportId, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
