using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;
public sealed class CarrierDashboardQueryHandlerTests
{
    [Fact]
    public async Task GetCarrierDashboardHandler_WhenCarrierDoesNotExist_ShouldReturnNotFoundWithoutLoadingMetricsAsync()
    {
        var fixture = new CarrierDashboardFixture();
        fixture.Carriers.Setup(x => x.GetCarrierAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrier?)null);

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Shipments.VerifyNoOtherCalls();
        fixture.Trips.VerifyNoOtherCalls();
        fixture.Analytics.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenCarrierExists_ShouldReturnCarrierProfileAsync()
    {
        var fixture = new CarrierDashboardFixture();

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Profile.FullName.Should().Be("Ahmed Ali");
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenStatsExist_ShouldAggregateAssignedShipmentStatusesAsync()
    {
        var fixture = new CarrierDashboardFixture();
        fixture.Stats[ShipmentStatuses.AssignedToPickUpCarrier] = 2;
        fixture.Stats[ShipmentStatuses.AssignedToDeliveryCarrier] = 3;
        fixture.Stats[ShipmentStatuses.OutForPickup] = 4;
        fixture.Stats[ShipmentStatuses.OutForDelivery] = 5;
        fixture.Stats[ShipmentStatuses.InTransit] = 6;

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.Data!.AssignedShipmentsCount.Should().Be(20);
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenTripsHaveMixedStatuses_ShouldReturnOnlyFiveActiveOrPlannedTripsAsync()
    {
        var fixture = new CarrierDashboardFixture();
        fixture.TripData.AddRange(Enumerable.Range(0, 7).Select(index => new CarrierTripDto
        {
            Id = Guid.NewGuid(),
            Status = index == 6 ? TripStatus.Completed : index % 2 == 0 ? TripStatus.Active : TripStatus.Planned,
            PlannedDate = DateTime.UtcNow.AddDays(index)
        }));

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.Data!.ActiveTrips.Should().HaveCount(5);
        result.Data.ActiveTrips.Should().OnlyContain(x =>
            x.Status == TripStatus.Active || x.Status == TripStatus.Planned);
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenRevenueIsMissing_ShouldReturnZeroRevenueAsync()
    {
        var fixture = new CarrierDashboardFixture();
        fixture.Analytics.Setup(x => x.GetCarrierRevenueAsync(
                fixture.Carrier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal?)null);

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.Data!.RevenueSummary.Should().Be(0m);
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenRecentShipmentsExist_ShouldReturnRepositoryPageDataAsync()
    {
        var fixture = new CarrierDashboardFixture();
        fixture.RecentShipments.Add(new RetrieveShipmentDto
        {
            Id = Guid.NewGuid(),
            TrackingNumber = "TN-1",
            Receiver = new UserSummaryDto { FullName = "Receiver" }
        });

        var result = await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        result.Data!.RecentShipments.Should().ContainSingle(x => x.TrackingNumber == "TN-1");
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenLoadingRecentShipments_ShouldRequestFirstTenItemsAsync()
    {
        var fixture = new CarrierDashboardFixture();

        await fixture.Handler.Handle(fixture.Query, CancellationToken.None);

        fixture.Shipments.Verify(x => x.GetCarrierShipmentsAsync(
            fixture.Carrier.Id,
            It.Is<CarrierShipmentFilterDto>(filter => filter.PageNumber == 1 && filter.PageSize == 10),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCarrierDashboardHandler_WhenCancellationTokenIsPassed_ShouldForwardItToEveryRepositoryAsync()
    {
        var fixture = new CarrierDashboardFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Query, cancellation.Token);

        fixture.Shipments.Verify(x => x.GetCarrierShipmentCountInStatusAsync(fixture.Carrier.Id, cancellation.Token), Times.Once);
        fixture.Trips.Verify(x => x.GetCarrierTripsAsync(fixture.Carrier.Id, cancellation.Token), Times.Once);
        fixture.Analytics.Verify(x => x.GetCarrierRevenueAsync(fixture.Carrier.Id, cancellation.Token), Times.Once);
    }

    private sealed class CarrierDashboardFixture
    {
        public Carrier Carrier { get; } = Carrier.Create(
            Guid.NewGuid(), "Ahmed", "Ali", "carrier@example.com", "01000000000", "Cairo", 1);
        public GetCarrierDashboardQuery Query { get; }
        public Dictionary<ShipmentStatuses, int> Stats { get; } = [];
        public List<CarrierTripDto> TripData { get; } = [];
        public List<RetrieveShipmentDto> RecentShipments { get; } = [];
        public Mock<ICarrierQueryRepository> Carriers { get; } = new();
        public Mock<ICarrierShipmentQueryRepository> Shipments { get; } = new();
        public Mock<ICarrierAnalyticsQueryRepository> Analytics { get; } = new();
        public Mock<ITripQueryRepository> Trips { get; } = new();
        public GetCarrierDashboardHandler Handler { get; }

        public CarrierDashboardFixture()
        {
            Query = new GetCarrierDashboardQuery(Carrier.Id);
            Carriers.Setup(x => x.GetCarrierAsync(
                    It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Carrier);
            Shipments.Setup(x => x.GetCarrierShipmentsAsync(
                    Carrier.Id, It.IsAny<CarrierShipmentFilterDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => PagedResult<RetrieveShipmentDto>.From(RecentShipments, RecentShipments.Count, 1, 10));
            Shipments.Setup(x => x.GetCarrierShipmentCountInStatusAsync(
                    Carrier.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Stats);
            Trips.Setup(x => x.GetCarrierTripsAsync(Carrier.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TripData);
            Analytics.Setup(x => x.GetCarrierRevenueAsync(Carrier.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(500m);
            Handler = new GetCarrierDashboardHandler(
                Carriers.Object,
                Shipments.Object,
                Analytics.Object,
                Trips.Object,
                Mock.Of<ILogger<GetCarrierDashboardHandler>>());
        }
    }
}
