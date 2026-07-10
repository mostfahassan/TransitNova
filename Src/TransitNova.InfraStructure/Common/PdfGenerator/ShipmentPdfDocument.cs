using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;

namespace TransitNova.InfraStructure.Common.PdfGenerator
{
    internal class ShipmentPdfDocument(RetrieveShipmentDto shipment) : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(PdfDocumentDesign.BaseTextStyle);

                page.Header().ComposeCompanyHeader(
                    "Shipment Report",
                    "Operational shipment manifest and status history",
                    shipment.Sender?.FullName ?? "TransitNova Operations",
                    PdfDocumentDesign.ValueOrDash(shipment.TrackingNumber));

                page.Content().PaddingVertical(18).Column(column =>
                {
                    column.Spacing(14);
                    column.Item().Element(ComposeShipmentHero);
                    column.Item().Element(ComposeRouteAndParties);
                    column.Item().Element(ComposePackageAndService);
                    column.Item().Element(ComposeStatusTimeline);
                    column.Item().Element(ComposeShipmentVerification);
                });

                page.Footer().ComposeFooter();
            });
        }

        private void ComposeShipmentHero(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().ComposeSummaryCard("Tracking Number", PdfDocumentDesign.ValueOrDash(shipment.TrackingNumber), "Primary shipment reference", PdfPalette.Blue);
                row.ConstantItem(10);
                row.RelativeItem().ComposeSummaryCard("Current Status", shipment.CurrentStatus.ToString(), "Live operational state", StatusColor(shipment.CurrentStatus.ToString()));
                row.ConstantItem(10);
                row.RelativeItem().ComposeSummaryCard("Shipping Cost", PdfDocumentDesign.Money(shipment.ShippingCost, shipment.Currency), "Booked shipment value", PdfPalette.Green);
                row.ConstantItem(10);
                row.RelativeItem().ComposeSummaryCard("ETA", PdfDocumentDesign.Date(shipment.EstimatedDeliveryDate), "Estimated delivery window", PdfPalette.Navy);
            });
        }

        private void ComposeRouteAndParties(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Route").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Pickup Address", shipment.PickupAddress);
                    column.Item().PaddingTop(7).ComposeInfoLine("Delivery Address", shipment.DeliveryAddress);
                    column.Item().PaddingTop(7).ComposeInfoLine("Transportation", shipment.TransportationMode.ToString());
                    column.Item().PaddingTop(7).ComposeInfoLine("Shipment Type", shipment.ShipmentType.ToString());
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Parties").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).Element(container => ComposeUserSummary(container, "Sender", shipment.Sender));
                    column.Item().PaddingTop(10).Element(container => ComposeUserSummary(container, "Receiver", shipment.Receiver));
                });
            });
        }

        private static void ComposeUserSummary(IContainer container, string label, UserSummaryDto? user)
        {
            container.BorderTop(1).BorderColor(PdfPalette.Border).PaddingTop(8).Column(column =>
            {
                column.Item().Text(label.ToUpperInvariant()).FontSize(7).SemiBold().FontColor(PdfPalette.Muted);
                column.Item().PaddingTop(3).Text(PdfDocumentDesign.ValueOrDash(user?.FullName)).FontSize(10).Bold().FontColor(PdfPalette.Navy);
                column.Item().Text(PdfDocumentDesign.ValueOrDash(user?.PhoneNumber)).FontSize(8).FontColor(PdfPalette.Muted);
                column.Item().Text(PdfDocumentDesign.ValueOrDash(user?.Email)).FontSize(8).FontColor(PdfPalette.Muted);
                column.Item().Text(PdfDocumentDesign.ValueOrDash(user?.CityName)).FontSize(8).FontColor(PdfPalette.Muted);
            });
        }

        private void ComposePackageAndService(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Package Specification").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Weight");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Length");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Width");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Height");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Bundle");
                    });

                    table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, 0)).Text($"{shipment.PackageSpecification.Weight:N2} kg");
                    table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, 0)).Text($"{shipment.PackageSpecification.Length:N2} cm");
                    table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, 0)).Text($"{shipment.PackageSpecification.Width:N2} cm");
                    table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, 0)).Text($"{shipment.PackageSpecification.Height:N2} cm");
                    table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, 0)).Text(shipment.PackageBundleId?.ToString() ?? "Standard billing");
                });
            });
        }

        private void ComposeStatusTimeline(IContainer container)
        {
            var states = shipment.ShipmentStates.OrderBy(state => state.ChangedAt).ToList();

            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Status Timeline").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Status");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Changed At");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Carrier / Handler");
                    });

                    if (states.Count == 0)
                    {
                        AddTimelineRow(table, 0, shipment.CurrentStatus.ToString(), shipment.CreatedAt, null);
                        return;
                    }

                    for (var index = 0; index < states.Count; index++)
                    {
                        var state = states[index];
                        AddTimelineRow(table, index, state.StatusType.ToString(), state.ChangedAt, state.Carrier);
                    }
                });
            });
        }

        private static void AddTimelineRow(TableDescriptor table, int index, string status, DateTime changedAt, UserSummaryDto? carrier)
        {
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(status).FontColor(StatusColor(status)).Bold();
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(changedAt.ToString("yyyy-MM-dd HH:mm"));
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(PdfDocumentDesign.ValueOrDash(carrier?.FullName));
        }

        private void ComposeShipmentVerification(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Height(78).ComposePlaceholder("Barcode", PdfDocumentDesign.ValueOrDash(shipment.TrackingNumber));
                row.ConstantItem(12);
                row.RelativeItem().Height(78).ComposePlaceholder("Operations Signature", "Approved by TransitNova operations");
                row.ConstantItem(12);
                row.RelativeItem().Height(78).ComposePlaceholder("Customer Signature", "Delivery confirmation placeholder");
            });
        }

        private static string StatusColor(string status)
        {
            var text = status.ToLowerInvariant();
            if (text.Contains("deliver") || text.Contains("warehouse")) return PdfPalette.Green;
            if (text.Contains("reject") || text.Contains("cancel")) return PdfPalette.Red;
            return PdfPalette.Blue;
        }
    }
}
