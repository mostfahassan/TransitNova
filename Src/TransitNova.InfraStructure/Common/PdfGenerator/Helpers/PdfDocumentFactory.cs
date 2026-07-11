using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.InfraStructure.Common.PdfGenerator.Helpers
{
    internal interface IPdfDocumentFactory
    {
        IDocument CreateInvoice(ShipmentPaymentInvoiceDto invoice);
        IDocument CreateShipment(RetrieveShipmentDto shipment);
        IDocument CreateDashboard(AdminDashboardDto dashboard);
    }

    internal sealed class PdfDocumentFactory : IPdfDocumentFactory
    {
        public IDocument CreateInvoice(ShipmentPaymentInvoiceDto invoice) => new InvoicePdfDocument(invoice);

        public IDocument CreateShipment(RetrieveShipmentDto shipment) => new ShipmentPdfDocument(shipment);

        public IDocument CreateDashboard(AdminDashboardDto dashboard) => new DashboardPdfDocument(dashboard);
    }
}
