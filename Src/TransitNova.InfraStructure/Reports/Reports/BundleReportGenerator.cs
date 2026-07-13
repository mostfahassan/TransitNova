using QuestPDF.Fluent;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Interface;
namespace TransitNova.InfraStructure.Reports.Reports
{
    internal sealed class BundleReportGenerator(IPaymentRepositoryQuery invoices, IFileStorageService fileStorage, IPdfDocumentFactory pdfDocumentFactory) : IReportGenerator
    {
        public string ReportKey => BundleReportContract.ReportKey;

        public async Task<string> GenerateReportAsync(string payloadJson, CancellationToken cancellationToken, Guid? reportId = null)
        {
            var contract = ReportPayloadSerializer.Deserialize<BundleReportContract>(payloadJson);
            if (contract.PaymentId == Guid.Empty)
            {
                throw new ArgumentException("PaymentId parameter is required.");
            }

            var invoice = await invoices.GetBundleInvoiceByPaymentIdAsync(contract.PaymentId, cancellationToken)
                ?? throw new InvalidOperationException("Bundle invoice not found.");

            var report = pdfDocumentFactory.CreateBundleInvoice(invoice);
            var reportBytes = report.GeneratePdf();
            var fileName = $"Bundle-Invoice-{invoice.InvoiceId ?? contract.PaymentId.ToString("N")}.pdf";

            return await fileStorage.SaveFileAsync(reportBytes, "Invoices", fileName, cancellationToken);
        }
    }
}