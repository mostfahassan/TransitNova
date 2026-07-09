using TransitNova.Domain.Enums.Reports;

namespace TransitNova.Api.Contracts.Reports
{
    public sealed class CreateReportRequest
    {
        public ReportType ReportType { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
