using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;

namespace TransitNova.InfraStructure.Common.PdfGenerator
{
    internal class DashboardPdfDocument(AdminDashboardDto dashboard) : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(PdfDocumentDesign.BaseTextStyle);

                page.Header().ComposeCompanyHeader(
                    "Executive Dashboard Report",
                    "Platform KPIs, revenue, operational health, and shipment activity",
                    "TransitNova Admin Console",
                    $"DASH-{DateTime.UtcNow:yyyyMMddHHmm}");

                page.Content().PaddingVertical(18).Column(column =>
                {
                    column.Spacing(14);
                    column.Item().Element(ComposeExecutiveSummary);
                    column.Item().Element(ComposeKpis);
                    column.Item().Element(ComposeOperationalHealth);
                    column.Item().Element(ComposeShipmentStatistics);
                    column.Item().Element(ComposeRecentShipments);
                    column.Item().Element(ComposeLeaderboardsAndActivities);
                });

                page.Footer().ComposeFooter();
            });
        }

        private void ComposeExecutiveSummary(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Executive Summary").FontSize(14).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(8).Text(
                    $"TransitNova currently tracks {dashboard.Kpis.TotalShipments:N0} shipments, {dashboard.Kpis.TotalUsers:N0} customers, {dashboard.Kpis.TotalCarriers:N0} carriers, and {dashboard.Kpis.ActiveTrips:N0} active trips. The current delivery success rate is {dashboard.OperationalHealth.DeliverySuccessRate:N1}% with an average carrier rating of {dashboard.OperationalHealth.AverageCarrierRating:N2}.")
                    .FontSize(9).LineHeight(1.35f).FontColor(PdfPalette.Slate);
            });
        }

        private void ComposeKpis(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("Key Performance Indicators").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(9).Column(cards =>
                {
                    cards.Spacing(10);
                    cards.Item().Row(row =>
                    {
                        row.RelativeItem().ComposeSummaryCard("Total Shipments", dashboard.Kpis.TotalShipments.ToString("N0"), "All-time shipments", PdfPalette.Blue);
                        row.ConstantItem(10);
                        row.RelativeItem().ComposeSummaryCard("Active Shipments", dashboard.Kpis.ActiveShipments.ToString("N0"), "Currently moving", PdfPalette.Navy);
                        row.ConstantItem(10);
                        row.RelativeItem().ComposeSummaryCard("Delivered", dashboard.Kpis.DeliveredShipments.ToString("N0"), "Completed shipments", PdfPalette.Green);
                    });
                    cards.Item().Row(row =>
                    {
                        row.RelativeItem().ComposeSummaryCard("Pending", dashboard.Kpis.PendingShipments.ToString("N0"), "Awaiting review", PdfPalette.Blue);
                        row.ConstantItem(10);
                        row.RelativeItem().ComposeSummaryCard("Active Carriers", dashboard.Kpis.ActiveCarriers.ToString("N0"), "Available capacity", PdfPalette.Green);
                        row.ConstantItem(10);
                        row.RelativeItem().ComposeSummaryCard("Monthly Revenue", dashboard.RevenueSummary.MonthlyRevenue.ToString("N2"), "Revenue this month", PdfPalette.Blue);
                    });
                });
            });
        }

        private void ComposeOperationalHealth(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Operational Health").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Available Carriers", dashboard.OperationalHealth.AvailableCarriers.ToString("N0"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Busy Carriers", dashboard.OperationalHealth.BusyCarriers.ToString("N0"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Active Managers", dashboard.OperationalHealth.ActiveOperationManagers.ToString("N0"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Average Rating", dashboard.OperationalHealth.AverageCarrierRating.ToString("N2"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Delivery Success", $"{dashboard.OperationalHealth.DeliverySuccessRate:N1}%");
                    column.Item().PaddingTop(6).ComposeInfoLine("Cancellation Rate", $"{dashboard.OperationalHealth.CancelledShipmentRate:N1}%");
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Revenue Summary").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).ComposeInfoLine("Total Revenue", dashboard.RevenueSummary.TotalRevenue.ToString("N2"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Monthly Revenue", dashboard.RevenueSummary.MonthlyRevenue.ToString("N2"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Weekly Revenue", dashboard.RevenueSummary.WeeklyRevenue.ToString("N2"));
                    column.Item().PaddingTop(6).ComposeInfoLine("Daily Revenue", dashboard.RevenueSummary.DailyRevenue.ToString("N2"));
                    column.Item().PaddingTop(16).Height(88).ComposePlaceholder("Revenue Trend", "Chart placeholder for weekly and monthly revenue movement");
                });
            });
        }

        private void ComposeShipmentStatistics(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Shipment Status Distribution").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Height(130).ComposePlaceholder("Status Chart", "Chart placeholder for shipment state distribution");
                    row.ConstantItem(14);
                    row.RelativeItem().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Status");
                            header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Count");
                        });

                        for (var index = 0; index < dashboard.ShipmentStatistics.Count; index++)
                        {
                            var stat = dashboard.ShipmentStatistics[index];
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(stat.Status.ToString());
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(stat.Count.ToString("N0"));
                        }
                    });
                });
            });
        }

        private void ComposeRecentShipments(IContainer container)
        {
            container.Element(PdfDocumentDesign.Card).Column(column =>
            {
                column.Item().Text("Recent Shipments").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Tracking");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Sender");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Receiver");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Status");
                        header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Cost");
                    });

                    var shipments = dashboard.RecentShipments.Take(12).ToList();
                    for (var index = 0; index < shipments.Count; index++)
                    {
                        AddShipmentRow(table, index, shipments[index]);
                    }
                });
            });
        }

        private static void AddShipmentRow(TableDescriptor table, int index, RetrieveShipmentDto shipment)
        {
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(PdfDocumentDesign.ValueOrDash(shipment.TrackingNumber));
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(PdfDocumentDesign.ValueOrDash(shipment.Sender?.FullName));
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(PdfDocumentDesign.ValueOrDash(shipment.Receiver?.FullName));
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(shipment.CurrentStatus.ToString()).FontColor(PdfPalette.Blue).Bold();
            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(PdfDocumentDesign.Money(shipment.ShippingCost, shipment.Currency));
        }

        private void ComposeLeaderboardsAndActivities(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Top Carriers").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Carrier");
                            header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Delivered");
                            header.Cell().Element(PdfDocumentDesign.TableHeader).AlignRight().Text("Rating");
                        });

                        for (var index = 0; index < dashboard.TopCarriers.Take(6).Count(); index++)
                        {
                            var carrier = dashboard.TopCarriers[index];
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(carrier.FullName);
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(carrier.DeliveredShipments.ToString("N0"));
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).AlignRight().Text(carrier.Rating.ToString("N2"));
                        }
                    });
                });

                row.ConstantItem(14);

                row.RelativeItem().Element(PdfDocumentDesign.Card).Column(column =>
                {
                    column.Item().Text("Recent Activities").FontSize(13).Bold().FontColor(PdfPalette.Navy);
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Title");
                            header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Description");
                            header.Cell().Element(PdfDocumentDesign.TableHeader).Text("Occurred");
                        });

                        for (var index = 0; index < dashboard.RecentActivities.Take(6).Count(); index++)
                        {
                            var activity = dashboard.RecentActivities[index];
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(activity.Title);
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(activity.Description);
                            table.Cell().Element(cell => PdfDocumentDesign.TableCell(cell, index)).Text(activity.OccurredAt.ToString("yyyy-MM-dd"));
                        }
                    });
                });
            });
        }
    }
}



