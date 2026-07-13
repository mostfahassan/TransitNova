using FluentAssertions;
using Hangfire;
using Moq;
using TransitNova.InfraStructure.BackgroundJobs;
using TransitNova.InfraStructure.Reports.Interface;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.BackgroundJobs;

public sealed class ReportGenerationJobTests
{
    [Fact]
    public async Task GenerateAsync_Should_ReturnWithoutResolvingGenerator_WhenRequestDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var generators = new Mock<IReportGeneratorFactory>(MockBehavior.Strict);
        var backgroundJobs = new Mock<IBackgroundJobClient>(MockBehavior.Strict);
        var sut = new ReportGenerationJob(fixture.Context, generators.Object, backgroundJobs.Object);

        var act = () => sut.GenerateAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().NotThrowAsync();
        generators.VerifyNoOtherCalls();
        backgroundJobs.VerifyNoOtherCalls();
    }
}
