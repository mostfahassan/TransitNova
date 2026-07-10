using QuestPDF.Fluent;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Reports.Reports
{
    internal sealed class DashboardReportGenerator(IAdminDashboard adminDashboard, IFileStorageService fileStorage, IPdfDocumentFactory pdfDocumentFactory) : IReportGenerator
    {
        public string ReportKey => DashboardReportContract.ReportKey;

        public async Task<string> GenerateReportAsync(string payloadJson, CancellationToken cancellationToken, Guid? reportId = null)
        {
            _ = ReportPayloadSerializer.Deserialize<DashboardReportContract>(payloadJson);

            var dashboard = await adminDashboard.BuildAsync(cancellationToken);
            var report = pdfDocumentFactory.CreateDashboard(dashboard);
            var reportBytes = report.GeneratePdf();
            var filePath = await fileStorage.SaveFileAsync(reportBytes, "Dashboards", $"Dashboard-{reportId?.ToString("N") ?? Guid.NewGuid().ToString("N")}.pdf", cancellationToken);

            return filePath;
        }
    }
}
