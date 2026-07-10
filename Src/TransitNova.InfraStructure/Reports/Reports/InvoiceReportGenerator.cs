using QuestPDF.Fluent;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Interface;
namespace TransitNova.InfraStructure.Reports.Reports
{
    internal sealed class InvoiceReportGenerator(IPaymentRepositoryQuery invoices, IFileStorageService fileStorage, IPdfDocumentFactory pdfDocumentFactory) : IReportGenerator
    {
        public string ReportKey => InvoiceReportContract.ReportKey;

        public async Task<string> GenerateReportAsync(string payloadJson, CancellationToken cancellationToken, Guid? reportId = null)
        {
            var contract = ReportPayloadSerializer.Deserialize<InvoiceReportContract>(payloadJson);
            if (contract.PaymentId == Guid.Empty)
            {
                throw new ArgumentException("PaymentId parameter is required.");
            }

            var invoice = await invoices.GetInvoiceByPaymentIdAsync(contract.PaymentId, cancellationToken)
                ?? throw new InvalidOperationException("Invoice not found.");

            var report = pdfDocumentFactory.CreateInvoice(invoice);
            var reportBytes = report.GeneratePdf();
            var filePath = await fileStorage.SaveFileAsync(reportBytes, "Invoices", $"Invoice-{invoice.InvoiceId}.pdf", cancellationToken);

            return filePath;
        }
    }
}
