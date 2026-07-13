using FluentAssertions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Common.PdfGenerator;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;

namespace TransitNova.InfraStructure.Tests.Reports;

public sealed class PdfDocumentRenderingTests
{
    static PdfDocumentRenderingTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public void ShipmentPdfDocument_Should_RenderOperationalManifest()
    {
        var shipment = Shipment("TN-PDF-100", ShipmentStatuses.Delivered);
        shipment.ShipmentStates =
        [
            new RetrieveShipmentStatusDto
            {
                StatusType = ShipmentStatuses.Approved,
                ChangedAt = DateTime.UtcNow.AddDays(-2),
                Carrier = new UserSummaryDto { FullName = "Ahmed Carrier" }
            },
            new RetrieveShipmentStatusDto
            {
                StatusType = ShipmentStatuses.Delivered,
                ChangedAt = DateTime.UtcNow,
                Carrier = new UserSummaryDto { FullName = "Ahmed Carrier" }
            }
        ];

        var bytes = new ShipmentPdfDocument(shipment).GeneratePdf();

        AssertPdf(bytes);
    }

    [Fact]
    public void ShipmentPdfDocument_Should_RenderFallbackTimeline_WhenHistoryIsEmpty()
    {
        var bytes = new ShipmentPdfDocument(Shipment("TN-PDF-EMPTY", ShipmentStatuses.Cancelled)).GeneratePdf();

        AssertPdf(bytes);
    }

    [Fact]
    public void InvoicePdfDocument_Should_RenderCustomerBillingDetails()
    {
        var invoice = new ShipmentPaymentInvoiceDto
        {
            InvoiceId = "INV-SHIPMENT-100",
            PaymentId = Guid.NewGuid(),
            ReferenceId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            ShipmentTrackingNumber = "TN-PDF-100",
            CustomerName = "Mona Ali",
            ShippingCost = 250m,
            Commission = 12.5m,
            TotalAmount = 262.5m,
            PaymentMethod = "CreditCard",
            Status = "Success",
            PaidAt = DateTime.UtcNow,
            Currency = Currency.EGP,
            Notes = "Paid successfully."
        };

        var bytes = new InvoicePdfDocument(invoice).GeneratePdf();

        AssertPdf(bytes);
    }

    [Fact]
    public void BundleInvoicePdfDocument_Should_RenderSubscriptionBillingDetails()
    {
        var invoice = new BundlePaymentInvoiceDto
        {
            InvoiceId = "INV-BUNDLE-100",
            PaymentId = Guid.NewGuid(),
            ReferenceId = Guid.NewGuid(),
            BundleId = Guid.NewGuid(),
            BundleName = "Pro Logistics",
            FullName = "Mona Ali",
            BundlePrice = 500m,
            Commission = 25m,
            TotalAmount = 525m,
            PaymentMethod = "PayPal",
            Status = "Paid",
            Currency = Currency.EGP,
            PaidAt = DateTime.UtcNow,
            SubscribedAt = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Notes = "Subscription active."
        };

        var bytes = new BundleInvoicePdfDocument(invoice).GeneratePdf();

        AssertPdf(bytes);
    }

    [Fact]
    public void DashboardPdfDocument_Should_RenderKpisTablesAndActivity()
    {
        var dashboard = new AdminDashboardDto
        {
            Kpis = new AdminKpiDto
            {
                TotalShipments = 100,
                ActiveShipments = 20,
                DeliveredShipments = 70,
                PendingShipments = 10,
                TotalUsers = 50,
                TotalCarriers = 8,
                ActiveCarriers = 6,
                ActiveTrips = 4
            },
            OperationalHealth = new AdminOperationalHealthDto
            {
                AvailableCarriers = 3,
                BusyCarriers = 5,
                ActiveOperationManagers = 2,
                AverageCarrierRating = 4.7m,
                DeliverySuccessRate = 94.5m,
                CancelledShipmentRate = 2.5m
            },
            RevenueSummary = new RevenueSummaryDto
            {
                TotalRevenue = 100_000m,
                MonthlyRevenue = 20_000m,
                WeeklyRevenue = 5_000m,
                DailyRevenue = 800m
            },
            ShipmentStatistics =
            [
                new ShipmentStatusStatDto { Status = ShipmentStatuses.Delivered, Count = 70 },
                new ShipmentStatusStatDto { Status = ShipmentStatuses.Pending, Count = 10 }
            ],
            RecentShipments = [Shipment("TN-DASH-100", ShipmentStatuses.InTransit)],
            TopCarriers = [new TopCarrierDto { CarrierId = Guid.NewGuid(), FullName = "Ahmed Carrier", DeliveredShipments = 25, Rating = 4.8m }],
            RecentActivities = [new AdminActivityDto { Title = "Shipment approved", Description = "TN-DASH-100", OccurredAt = DateTime.UtcNow, PerformedBy = "Operations" }]
        };

        var bytes = new DashboardPdfDocument(dashboard).GeneratePdf();

        AssertPdf(bytes);
    }

    [Theory]
    [InlineData(null, "-")]
    [InlineData("", "-")]
    [InlineData("value", "value")]
    public void PdfDocumentDesign_Should_NormalizeOptionalValues(string? value, string expected)
    {
        PdfDocumentDesign.ValueOrDash(value).Should().Be(expected);
    }

    [Fact]
    public void PdfDocumentDesign_Should_FormatIdentifiersDatesAndMoney()
    {
        var id = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var date = new DateTime(2026, 7, 13, 10, 30, 0, DateTimeKind.Utc);

        PdfDocumentDesign.ShortId(id).Should().Be("12345678");
        PdfDocumentDesign.ShortId(Guid.Empty).Should().Be("-");
        PdfDocumentDesign.Date(date).Should().Be("2026-07-13 10:30");
        PdfDocumentDesign.Date(null).Should().Be("Pending");
        PdfDocumentDesign.Money(125.5m, Currency.EGP).Should().Contain("125.50").And.EndWith("EGP");
    }

    private static RetrieveShipmentDto Shipment(string trackingNumber, ShipmentStatuses status) => new()
    {
        Id = Guid.NewGuid(),
        ReceiverId = Guid.NewGuid(),
        SenderId = Guid.NewGuid(),
        TrackingNumber = trackingNumber,
        CurrentStatus = status,
        ShippingCost = 250m,
        Currency = Currency.EGP,
        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(2),
        CreatedAt = DateTime.UtcNow.AddDays(-2),
        TransportationMode = TransportationMode.Land,
        ShipmentType = enShipmentType.Standard,
        PickupAddress = new AddressDto { MainAddress = "Cairo", Street = "Pickup Street" },
        DeliveryAddress = new AddressDto { MainAddress = "Alexandria", Street = "Delivery Street" },
        PackageSpecification = new PackageSpecificationDto { Weight = 5, Length = 10, Width = 10, Height = 10 },
        Sender = new UserSummaryDto { FullName = "Mona Sender", Email = "sender@example.com", PhoneNumber = "+201001234567", CityName = "Cairo" },
        Receiver = new UserSummaryDto { FullName = "Ali Receiver", Email = "receiver@example.com", PhoneNumber = "+201009876543", CityName = "Alexandria" }
    };

    private static void AssertPdf(byte[] bytes)
    {
        bytes.Should().HaveCountGreaterThan(1_000);
        System.Text.Encoding.ASCII.GetString(bytes, 0, 4).Should().Be("%PDF");
    }
}
