using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
namespace TransitNova.InfraStructure.Common.PdfGenerator
{
    internal class InvoicePdfDocument(PaymentInvoiceDto invoice) : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(PdfDocumentDesign.BaseTextStyle);

                page.Header().ComposeCompanyHeader(
                    "Payment Invoice",
                    "Customer invoice and shipment billing summary",
                    invoice.CustomerName ?? "TransitNova Billing",
                    invoice.InvoiceId ?? PdfDocumentDesign.ShortId(invoice.PaymentId));

                page.Content().PaddingVertical(18).Column(column =>
                {
                    column.Spacing(14);
                    column.Item().Element(ComposeInvoiceHero);
                    column.Item().Element(ComposePartiesAndShipment);
                    column.Item().Element(ComposeInvoiceItems);
                    column.Item().Element(ComposeTotalsAndVerification);
                    column.Item().Element(ComposeNotes);
                });

                page.Footer().ComposeFooter();
            });
        }

        private void ComposeInvoiceHero(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Invoice Number").FontSize(8).SemiBold().FontColor(PdfPalette.Muted);
                    column.Item().PaddingTop(4).Text(invoice.InvoiceId ?? $"INV-{PdfDocumentDesign.ShortId(invoice.PaymentId)}")
                        .FontSize(24).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(7).Text($"Payment ID: {invoice.PaymentId}")
                        .FontSize(8).FontColor(PdfPalette.Muted);
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Payment Status").FontSize(8).SemiBold().FontColor(PdfPalette.Muted);
                    column.Item().PaddingTop(4).Text(invoice.Status.ToString()).FontSize(22).Bold()
                        .FontColor(StatusColor(invoice.Status.ToString()));
                    column.Item().PaddingTop(7).Text($"Paid at: {PdfDocumentDesign.Date(invoice.PaidAt)}")
                        .FontSize(8).FontColor(PdfPalette.Muted);
                });
            });
        }

        private void ComposePartiesAndShipment(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Customer Information").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Customer", PdfDocumentDesign.ValueOrDash(invoice.CustomerName));
                    column.Item().PaddingTop(5).ComposeInfoLine("Billing Address", "Customer billing address on account");
                    column.Item().PaddingTop(5).ComposeInfoLine("Payment Method", invoice.PaymentMethod.ToString());
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Shipment Information").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Tracking Number", PdfDocumentDesign.ValueOrDash(invoice.ShipmentTrackingNumber));
                    column.Item().PaddingTop(5).ComposeInfoLine("Shipment ID", invoice.ShipmentId.ToString());
                    column.Item().PaddingTop(5).ComposeInfoLine("Carrier", "Assigned carrier");
                    column.Item().PaddingTop(5).ComposeInfoLine("Shipping Address", "Shipment destination address");
                });
            });
        }

        private void ComposeInvoiceItems(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Invoice Items").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Description");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Qty");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Unit Price");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Amount");
                    });

                    AddInvoiceRow(table, 0, "Shipment transportation charge", 1, invoice.ShippingCost);
                    AddInvoiceRow(table, 1, "TransitNova platform commission", 1, invoice.Commission);
                    AddInvoiceRow(table, 2, "Tax", 1, 0);
                    AddInvoiceRow(table, 3, "Discount", 1, 0);
                });
            });
        }

        private void ComposeTotalsAndVerification(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Verification").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).Row(inner =>
                    {
                        inner.RelativeItem().Height(86).ComposePlaceholder("QR Code", "Scan placeholder");
                        inner.ConstantItem(10);
                        inner.RelativeItem().Height(86).ComposePlaceholder("Barcode", PdfDocumentDesign.ValueOrDash(invoice.ShipmentTrackingNumber));
                    });
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Totals").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Subtotal", PdfDocumentDesign.Money(invoice.ShippingCost + invoice.Commission, invoice.Currency));
                    column.Item().PaddingTop(5).ComposeInfoLine("Tax", PdfDocumentDesign.Money(0, invoice.Currency));
                    column.Item().PaddingTop(5).ComposeInfoLine("Discount", PdfDocumentDesign.Money(0, invoice.Currency));
                    column.Item().PaddingTop(10).BorderTop(1).BorderColor(PdfPalette.Border).PaddingTop(10)
                        .Row(total =>
                        {
                            total.RelativeItem().Text("Grand Total").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                            total.AutoItem().Text(PdfDocumentDesign.Money(invoice.TotalAmount, invoice.Currency))
                                .FontSize(16).Bold().FontColor(PdfPalette.Blue);
                        });
                });
            });
        }

        private void ComposeNotes(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Notes").FontSize(12).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(6).Text(PdfDocumentDesign.ValueOrDash(invoice.Notes))
                    .FontSize(9).FontColor(PdfPalette.Slate);
            });
        }

        private void AddInvoiceRow(TableDescriptor table, int index, string description, int quantity, decimal amount)
        {
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(description);
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(quantity.ToString());
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(PdfDocumentDesign.Money(amount, invoice.Currency));
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(PdfDocumentDesign.Money(amount * quantity, invoice.Currency));
        }

        private static string StatusColor(string status)
        {
            var text = status.ToLowerInvariant();
            if (text.Contains("success") || text.Contains("paid")) return PdfPalette.Green;
            if (text.Contains("fail") || text.Contains("declin")) return PdfPalette.Red;
            return PdfPalette.Blue;
        }
    }
}
