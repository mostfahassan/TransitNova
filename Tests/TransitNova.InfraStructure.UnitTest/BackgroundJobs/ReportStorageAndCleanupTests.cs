using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Quartz;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.InfraStructure.BackgroundJobs;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.Interfaces.Implementation;

namespace TransitNova.InfraStructure.Tests.BackgroundJobs;

public sealed class ReportStorageAndCleanupTests
{
    [Fact]
    public async Task FileStorageService_Should_SaveReadAndDeleteFileAsync()
    {
        var root = Path.Combine(Path.GetTempPath(), $"transitnova-storage-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(x => x.ContentRootPath).Returns(root);
            var sut = new FileStorageService(environment.Object, NullLogger<FileStorageService>.Instance);
            var content = "TransitNova report"u8.ToArray();

            var path = await sut.SaveFileAsync(content, "Invoices", "invoice.pdf", CancellationToken.None);

            File.Exists(path).Should().BeTrue();
            (await File.ReadAllBytesAsync(path)).Should().Equal(content);
            (await sut.DeleteFileAsync(path, CancellationToken.None)).Should().BeTrue();
            File.Exists(path).Should().BeFalse();
            (await sut.DeleteFileAsync(path, CancellationToken.None)).Should().BeTrue();
            (await sut.DeleteFileAsync(string.Empty, CancellationToken.None)).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task FileStorageService_Should_HonorCancellationWhileSavingAsync()
    {
        var root = Path.Combine(Path.GetTempPath(), $"transitnova-storage-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(x => x.ContentRootPath).Returns(root);
            var sut = new FileStorageService(environment.Object, NullLogger<FileStorageService>.Instance);
            using var source = new CancellationTokenSource();
            source.Cancel();

            var act = () => sut.SaveFileAsync([1, 2, 3], "Reports", "cancelled.pdf", source.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ReportCleanupJob_Should_ReturnWithoutWork_WhenNoReportsAreExpiredAsync()
    {
        var fixture = new CleanupFixture();
        fixture.Queries.Setup(x => x.ReportCleanableData(CancellationToken.None))
            .ReturnsAsync([]);

        await fixture.CreateSut().Execute(fixture.Context.Object);

        fixture.Storage.Verify(x => x.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.Commands.Verify(x => x.ExpireReportsBulkAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReportCleanupJob_Should_ExpireOnlyFilesDeletedSuccessfullyAsync()
    {
        var fixture = new CleanupFixture();
        var deleted = new ReportClanableData(Guid.NewGuid(), "deleted.pdf");
        var retained = new ReportClanableData(Guid.NewGuid(), "retained.pdf");
        var failed = new ReportClanableData(Guid.NewGuid(), "failed.pdf");
        fixture.Queries.Setup(x => x.ReportCleanableData(CancellationToken.None))
            .ReturnsAsync([deleted, retained, failed]);
        fixture.Storage.Setup(x => x.DeleteFileAsync(deleted.FilePath, CancellationToken.None)).ReturnsAsync(true);
        fixture.Storage.Setup(x => x.DeleteFileAsync(retained.FilePath, CancellationToken.None)).ReturnsAsync(false);
        fixture.Storage.Setup(x => x.DeleteFileAsync(failed.FilePath, CancellationToken.None)).ThrowsAsync(new IOException("locked"));
        IReadOnlyCollection<Guid>? expiredIds = null;
        fixture.Commands.Setup(x => x.ExpireReportsBulkAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .Callback<IEnumerable<Guid>, CancellationToken>((ids, _) => expiredIds = ids.ToArray())
            .Returns(Task.CompletedTask);

        await fixture.CreateSut().Execute(fixture.Context.Object);

        expiredIds.Should().Equal(deleted.Id);
    }

    [Fact]
    public async Task ReportCleanupJob_Should_PropagateBulkPersistenceFailureAsync()
    {
        var fixture = new CleanupFixture();
        var report = new ReportClanableData(Guid.NewGuid(), "deleted.pdf");
        fixture.Queries.Setup(x => x.ReportCleanableData(CancellationToken.None)).ReturnsAsync([report]);
        fixture.Storage.Setup(x => x.DeleteFileAsync(report.FilePath, CancellationToken.None)).ReturnsAsync(true);
        fixture.Commands.Setup(x => x.ExpireReportsBulkAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));

        var act = () => fixture.CreateSut().Execute(fixture.Context.Object);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }

    private sealed class CleanupFixture
    {
        internal Mock<IReportRequestCommands> Commands { get; } = new();
        internal Mock<IReportRequestQueryRepository> Queries { get; } = new();
        internal Mock<IFileStorageService> Storage { get; } = new();
        internal Mock<IJobExecutionContext> Context { get; } = new();

        internal CleanupFixture()
        {
            Context.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
        }

        internal ReportCleanupJob CreateSut() => new(
            Commands.Object,
            Queries.Object,
            Storage.Object,
            NullLogger<ReportCleanupJob>.Instance);
    }
}
