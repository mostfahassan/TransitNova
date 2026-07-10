using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Reports;

namespace TransitNova.InfraStructure.Tests.Reports;

public sealed class ReportGeneratorTests
{
    static ReportGeneratorTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public async Task DashboardReportGenerator_Should_BuildDashboardPdf_And_SaveItAsync()
    {
        var dashboard = new AdminDashboardDto();
        var adminDashboard = new Mock<IAdminDashboard>();
        adminDashboard.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        var pdfDocumentFactory = new TestPdfDocumentFactory
        {
            DashboardDocument = new StubDocument("dashboard-report")
        };

        byte[]? savedBytes = null;
        string? savedFolder = null;
        string? savedFileName = null;

        var fileStorage = new Mock<IFileStorageService>();
        fileStorage.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<byte[], string, string, CancellationToken>((content, folder, fileName, _) =>
            {
                savedBytes = content;
                savedFolder = folder;
                savedFileName = fileName;
            })
            .ReturnsAsync("reports/dashboard.pdf");

        var generator = new DashboardReportGenerator(adminDashboard.Object, fileStorage.Object, pdfDocumentFactory);
        var reportId = Guid.NewGuid();
        var payloadJson = ReportPayloadSerializer.Serialize(new DashboardReportContract());

        var result = await generator.GenerateReportAsync(payloadJson, CancellationToken.None, reportId);

        result.Should().Be("reports/dashboard.pdf");
        savedBytes.Should().NotBeNull();
        savedBytes.Should().NotBeEmpty();
        savedFolder.Should().Be("Dashboards");
        savedFileName.Should().Be($"Dashboard-{reportId:N}.pdf");
        pdfDocumentFactory.DashboardArgument.Should().BeSameAs(dashboard);
        adminDashboard.Verify(x => x.BuildAsync(CancellationToken.None), Times.Once);
        fileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), "Dashboards", $"Dashboard-{reportId:N}.pdf", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ShipmentAnalyticsReportGenerator_Should_BuildShipmentPdf_And_SaveItAsync()
    {
        var shipmentId = Guid.NewGuid();
        var shipment = new RetrieveShipmentDto
        {
            Id = shipmentId,
            TrackingNumber = "TN-4091",
            Sender = new UserSummaryDto { FullName = "Mostafa" },
            Receiver = new UserSummaryDto { FullName = "Ahmed" }
        };

        var shipments = new Mock<IShipmentQueryRepository>();
        shipments.Setup(x => x.GetShipmentAsync(It.IsAny<Expression<Func<Shipment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shipment);

        var pdfDocumentFactory = new TestPdfDocumentFactory
        {
            ShipmentDocument = new StubDocument("shipment-report")
        };

        byte[]? savedBytes = null;
        string? savedFolder = null;
        string? savedFileName = null;

        var fileStorage = new Mock<IFileStorageService>();
        fileStorage.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<byte[], string, string, CancellationToken>((content, folder, fileName, _) =>
            {
                savedBytes = content;
                savedFolder = folder;
                savedFileName = fileName;
            })
            .ReturnsAsync("reports/shipment.pdf");

        var generator = new ShipmentAnalyticsReportGenerator(shipments.Object, fileStorage.Object, pdfDocumentFactory);
        var payloadJson = ReportPayloadSerializer.Serialize(new ShipmentReportContract { ShipmentId = shipmentId });

        var result = await generator.GenerateReportAsync(payloadJson, CancellationToken.None, Guid.NewGuid());

        result.Should().Be("reports/shipment.pdf");
        savedBytes.Should().NotBeNull();
        savedBytes.Should().NotBeEmpty();
        savedFolder.Should().Be("Shipments");
        savedFileName.Should().Be("Shipment-TN-4091.pdf");
        pdfDocumentFactory.ShipmentArgument.Should().BeSameAs(shipment);
        shipments.Verify(x => x.GetShipmentAsync(It.IsAny<Expression<Func<Shipment, bool>>>(), CancellationToken.None), Times.Once);
        fileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), "Shipments", "Shipment-TN-4091.pdf", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ShipmentAnalyticsReportGenerator_Should_Throw_When_ShipmentId_Is_Empty()
    {
        var shipments = new Mock<IShipmentQueryRepository>(MockBehavior.Strict);
        var fileStorage = new Mock<IFileStorageService>(MockBehavior.Strict);
        var generator = new ShipmentAnalyticsReportGenerator(shipments.Object, fileStorage.Object, new TestPdfDocumentFactory());
        var payloadJson = ReportPayloadSerializer.Serialize(new ShipmentReportContract());

        var action = () => generator.GenerateReportAsync(payloadJson, CancellationToken.None, Guid.NewGuid());

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("ShipmentId parameter is required.");
    }

    [Fact]
    public async Task ShipmentAnalyticsReportGenerator_Should_Throw_When_Shipment_Does_Not_Exist()
    {
        var shipmentId = Guid.NewGuid();
        var shipments = new Mock<IShipmentQueryRepository>();
        shipments.Setup(x => x.GetShipmentAsync(It.IsAny<Expression<Func<Shipment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RetrieveShipmentDto?)null);

        var fileStorage = new Mock<IFileStorageService>(MockBehavior.Strict);
        var generator = new ShipmentAnalyticsReportGenerator(shipments.Object, fileStorage.Object, new TestPdfDocumentFactory());
        var payloadJson = ReportPayloadSerializer.Serialize(new ShipmentReportContract { ShipmentId = shipmentId });

        var action = () => generator.GenerateReportAsync(payloadJson, CancellationToken.None, Guid.NewGuid());

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Shipment not found.");
    }

    private sealed class TestPdfDocumentFactory : IPdfDocumentFactory
    {
        public IDocument InvoiceDocument { get; init; } = new StubDocument("invoice-report");
        public IDocument ShipmentDocument { get; init; } = new StubDocument("shipment-report");
        public IDocument DashboardDocument { get; init; } = new StubDocument("dashboard-report");
        public PaymentInvoiceDto? InvoiceArgument { get; private set; }
        public RetrieveShipmentDto? ShipmentArgument { get; private set; }
        public AdminDashboardDto? DashboardArgument { get; private set; }

        public IDocument CreateInvoice(PaymentInvoiceDto invoice)
        {
            InvoiceArgument = invoice;
            return InvoiceDocument;
        }

        public IDocument CreateShipment(RetrieveShipmentDto shipment)
        {
            ShipmentArgument = shipment;
            return ShipmentDocument;
        }

        public IDocument CreateDashboard(AdminDashboardDto dashboard)
        {
            DashboardArgument = dashboard;
            return DashboardDocument;
        }
    }

    private sealed class StubDocument(string title) : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(16);
                page.Content().Text(title);
            });
        }
    }
}
