using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.Features.Admin.Handler;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;

namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;

public sealed class AdminDashboardQueryHandlerTests
{
    [Fact]
    public async Task GetAdminDashboardHandler_WhenServiceBuildsDashboard_ShouldReturnSuccessAsync()
    {
        var fixture = new AdminDashboardFixture();

        var result = await fixture.Handler.Handle(new GetAdminDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetAdminDashboardHandler_WhenServiceBuildsDashboard_ShouldReturnSameDashboardInstanceAsync()
    {
        var fixture = new AdminDashboardFixture();

        var result = await fixture.Handler.Handle(new GetAdminDashboardQuery(), CancellationToken.None);

        result.Data.Should().BeSameAs(fixture.Dashboard);
    }

    [Fact]
    public async Task GetAdminDashboardHandler_WhenServiceBuildsDashboard_ShouldPreserveKpisAsync()
    {
        var fixture = new AdminDashboardFixture();

        var result = await fixture.Handler.Handle(new GetAdminDashboardQuery(), CancellationToken.None);

        result.Data!.Kpis.TotalShipments.Should().Be(42);
        result.Data.Kpis.DeliveredShipments.Should().Be(17);
    }

    [Fact]
    public async Task GetAdminDashboardHandler_WhenServiceBuildsDashboard_ShouldPreserveRevenueAsync()
    {
        var fixture = new AdminDashboardFixture();

        var result = await fixture.Handler.Handle(new GetAdminDashboardQuery(), CancellationToken.None);

        result.Data!.RevenueSummary.TotalRevenue.Should().Be(12_500m);
    }

    [Fact]
    public async Task GetAdminDashboardHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var fixture = new AdminDashboardFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(new GetAdminDashboardQuery(), cancellation.Token);

        fixture.Service.Verify(x => x.BuildAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task GetAdminDashboardHandler_WhenDashboardServiceFails_ShouldPropagateExceptionAsync()
    {
        var fixture = new AdminDashboardFixture();
        fixture.Service.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("dashboard unavailable"));

        var act = () => fixture.Handler.Handle(new GetAdminDashboardQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("dashboard unavailable");
    }

    private sealed class AdminDashboardFixture
    {
        public AdminDashboardDto Dashboard { get; } = new()
        {
            Kpis = new AdminKpiDto { TotalShipments = 42, DeliveredShipments = 17 },
            RevenueSummary = new RevenueSummaryDto { TotalRevenue = 12_500m }
        };
        public Mock<IAdminDashboard> Service { get; } = new();
        public GetAdminDashboardHandler Handler { get; }

        public AdminDashboardFixture()
        {
            Service.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Dashboard);
            Handler = new GetAdminDashboardHandler(
                Service.Object,
                Mock.Of<ILogger<GetAdminDashboardHandler>>());
        }
    }
}
