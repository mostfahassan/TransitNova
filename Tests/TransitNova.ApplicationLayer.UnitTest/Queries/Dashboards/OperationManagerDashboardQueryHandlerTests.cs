using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;

public sealed class OperationManagerDashboardQueryHandlerTests
{
    [Fact]
    public async Task GetOperationManagerDashboardHandler_WhenRepositoryReturnsData_ShouldReturnSuccessAsync()
    {
        var fixture = new OperationDashboardFixture();

        var result = await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetOperationManagerDashboardHandler_WhenRepositoryReturnsData_ShouldUseCurrentPageCountAsTotalShipmentsAsync()
    {
        var fixture = new OperationDashboardFixture();
        fixture.ShipmentData.AddRange(CreateShipments(3));

        var result = await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), CancellationToken.None);

        result.Data!.TotalShipments.Should().Be(3);
    }

    [Theory]
    [InlineData(ShipmentStatuses.Pending, 4)]
    [InlineData(ShipmentStatuses.Delivered, 7)]
    public async Task GetOperationManagerDashboardHandler_WhenStatusCountExists_ShouldMapStatusCountAsync(
        ShipmentStatuses status,
        int count)
    {
        var fixture = new OperationDashboardFixture();
        fixture.Stats[status] = count;

        var result = await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), CancellationToken.None);

        var actual = status == ShipmentStatuses.Pending
            ? result.Data!.PendingShipments
            : result.Data!.DeliveredShipments;
        actual.Should().Be(count);
    }

    [Fact]
    public async Task GetOperationManagerDashboardHandler_WhenAssignedStatusesExist_ShouldAggregateActiveShipmentsAsync()
    {
        var fixture = new OperationDashboardFixture();
        fixture.Stats[ShipmentStatuses.AssignedToPickUpCarrier] = 1;
        fixture.Stats[ShipmentStatuses.AssignedToDeliveryCarrier] = 2;
        fixture.Stats[ShipmentStatuses.OutForPickup] = 3;
        fixture.Stats[ShipmentStatuses.OutForDelivery] = 4;
        fixture.Stats[ShipmentStatuses.InWarehouse] = 5;
        fixture.Stats[ShipmentStatuses.InTransit] = 6;

        var result = await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), CancellationToken.None);

        result.Data!.ActiveShipments.Should().Be(21);
    }

    [Fact]
    public async Task GetOperationManagerDashboardHandler_WhenManyShipmentsExist_ShouldLimitRecentActivityToFiveAsync()
    {
        var fixture = new OperationDashboardFixture();
        fixture.ShipmentData.AddRange(CreateShipments(8));

        var result = await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), CancellationToken.None);

        result.Data!.RecentActivity.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetOperationManagerDashboardHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var fixture = new OperationDashboardFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(new GetOperationManagerDashboardQuery(), cancellation.Token);

        fixture.Repository.Verify(x => x.FilterAsync(
            It.Is<ShipmentFilterDto>(filter => filter.PageNumber == 1 && filter.PageSize == 8),
            cancellation.Token), Times.Once);
        fixture.Repository.Verify(x => x.GetShipmentCountInStatusAsync(cancellation.Token), Times.Once);
    }

    private static IEnumerable<RetrieveShipmentDto> CreateShipments(int count) =>
        Enumerable.Range(1, count).Select(index => new RetrieveShipmentDto
        {
            Id = Guid.NewGuid(),
            TrackingNumber = $"TN-{index}",
            CurrentStatus = ShipmentStatuses.Pending,
            CreatedAt = DateTime.UtcNow.AddMinutes(-index),
            Receiver = new UserSummaryDto { FullName = $"Receiver {index}" }
        });

    private sealed class OperationDashboardFixture
    {
        public List<RetrieveShipmentDto> ShipmentData { get; } = [];
        public Dictionary<ShipmentStatuses, int> Stats { get; } = [];
        public Mock<IShipmentQueryRepository> Repository { get; } = new();
        public GetOperationManagerDashboardHandler Handler { get; }

        public OperationDashboardFixture()
        {
            Repository.Setup(x => x.FilterAsync(
                    It.IsAny<ShipmentFilterDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => PagedResult<RetrieveShipmentDto>.From(
                    ShipmentData, ShipmentData.Count, 1, 8));
            Repository.Setup(x => x.GetShipmentCountInStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Stats);
            Handler = new GetOperationManagerDashboardHandler(
                Repository.Object,
                Mock.Of<ILogger<GetOperationManagerDashboardHandler>>());
        }
    }
}
