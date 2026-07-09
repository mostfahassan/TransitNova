using TransitNova.Domain.Enums.Reports;

namespace TransitNova.BusinessLayer.DTOs.Reports
{
    public class ReportCommand
    {
        public ReportType ReportType { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
        public Guid RequestedBy { get; set; } = Guid.Empty;
    }
}
