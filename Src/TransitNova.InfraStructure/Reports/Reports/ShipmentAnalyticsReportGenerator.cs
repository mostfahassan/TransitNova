using QuestPDF.Fluent;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Reports.Reports
{
    internal sealed class ShipmentAnalyticsReportGenerator(IShipmentQueryRepository shipments, IFileStorageService fileStorage, IPdfDocumentFactory pdfDocumentFactory) : IReportGenerator
    {
        public string ReportKey => ShipmentReportContract.ReportKey;

        public async Task<string> GenerateReportAsync(string payloadJson, CancellationToken cancellationToken, Guid? reportId = null)
        {
            var contract = ReportPayloadSerializer.Deserialize<ShipmentReportContract>(payloadJson);
            if (contract.ShipmentId == Guid.Empty)
            {
                throw new ArgumentException("ShipmentId parameter is required.");
            }

            var shipment = await shipments.GetShipmentAsync(entity => entity.Id == contract.ShipmentId, cancellationToken)
                ?? throw new InvalidOperationException("Shipment not found.");

            var report = pdfDocumentFactory.CreateShipment(shipment);
            var reportBytes = report.GeneratePdf();
            var shipmentReference = string.IsNullOrWhiteSpace(shipment.TrackingNumber)
                ? contract.ShipmentId.ToString("N")
                : shipment.TrackingNumber;

            return await fileStorage.SaveFileAsync(reportBytes, "Shipments", $"Shipment-{shipmentReference}.pdf", cancellationToken);
        }
    }
}
