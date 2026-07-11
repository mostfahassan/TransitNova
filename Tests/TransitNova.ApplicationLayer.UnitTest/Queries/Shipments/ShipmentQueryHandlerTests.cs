using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.ApplicationLayer.Tests.Queries.Shipments;
public sealed class ShipmentQueryHandlerTests
{
    [Fact]
    public async Task GetShipmentByIdHandler_WhenShipmentExists_ShouldReturnRepositoryDtoAsync()
    {
        var repository = new Mock<IShipmentQueryRepository>();
        var shipmentId = Guid.NewGuid();
        var dto = ShipmentDto(shipmentId, "TN-100");
        repository.Setup(x => x.CreateShipmentForUserAsync(shipmentId, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        var handler = new GetShipmentByIdHandler(Mock.Of<ILogger<GetShipmentByIdHandler>>(), repository.Object);

        var result = await handler.Handle(new GetShipmentByIdQuery(shipmentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetShipmentByIdHandler_WhenShipmentDoesNotExist_ShouldReturnNotFoundAsync()
    {
        var repository = new Mock<IShipmentQueryRepository>();
        repository.Setup(x => x.CreateShipmentForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((RetrieveShipmentDto?)null);
        var handler = new GetShipmentByIdHandler(Mock.Of<ILogger<GetShipmentByIdHandler>>(), repository.Object);

        var result = await handler.Handle(new GetShipmentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Error!.Message.Should().Be("Shipment Not Found");
    }

    [Fact]
    public async Task GetShipmentByIdHandler_WhenCancellationTokenIsProvided_ShouldForwardTokenToRepositoryAsync()
    {
        var repository = new Mock<IShipmentQueryRepository>();
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var shipmentId = Guid.NewGuid();
        repository.Setup(x => x.CreateShipmentForUserAsync(shipmentId, token)).ReturnsAsync(ShipmentDto(shipmentId, "TN-101"));
        var handler = new GetShipmentByIdHandler(Mock.Of<ILogger<GetShipmentByIdHandler>>(), repository.Object);

        await handler.Handle(new GetShipmentByIdQuery(shipmentId), token);

        repository.Verify(x => x.CreateShipmentForUserAsync(shipmentId, token), Times.Once);
    }

    [Fact]
    public async Task GetShipmentStatisticsHandler_WhenStatisticsExist_ShouldReturnSameStatusCountsAsync()
    {
        var repository = new Mock<IShipmentQueryRepository>();
        var statistics = new Dictionary<ShipmentStatuses, int>
        {
            [ShipmentStatuses.Pending] = 4,
            [ShipmentStatuses.Delivered] = 9
        };
        repository.Setup(x => x.GetShipmentCountInStatusAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statistics);
        var handler = new GetShipmentStatisticsHandler(Mock.Of<ILogger<GetShipmentStatisticsHandler>>(), repository.Object);

        var result = await handler.Handle(new GetShipmentStatisticsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(statistics);
    }

    [Fact]
    public async Task GetShipmentStatisticsHandler_WhenNoStatisticsExist_ShouldReturnSuccessWithEmptyDictionaryAsync()
    {
        var repository = new Mock<IShipmentQueryRepository>();
        repository.Setup(x => x.GetShipmentCountInStatusAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetShipmentStatisticsHandler(Mock.Of<ILogger<GetShipmentStatisticsHandler>>(), repository.Object);

        var result = await handler.Handle(new GetShipmentStatisticsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Theory]
    [InlineData(true, ResultStatus.Success)]
    [InlineData(false, ResultStatus.NotFound)]
    public async Task TrackShipmentQueryHandler_WhenRepositoryResultIsKnown_ShouldReturnExpectedStatusAsync(bool exists, ResultStatus expectedStatus)
    {
        var repository = new Mock<IShipmentQueryRepository>();
        repository.Setup(x => x.GetByTrackingNumberAsync("TN-TRACK", It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? new RetrieveShipmentSummaryDto { Id = Guid.NewGuid(), TrackingNumber = "TN-TRACK" } : null);
        var handler = new TrackShipmentQueryHandler(Mock.Of<ILogger<TrackShipmentQueryHandler>>(), repository.Object);

        var result = await handler.Handle(new TrackShipmentQuery("TN-TRACK"), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
        if (exists)
            result.Data!.TrackingNumber.Should().Be("TN-TRACK");
    }

    [Theory]
    [InlineData(true, ResultStatus.Success)]
    [InlineData(false, ResultStatus.NotFound)]
    public async Task GetUserShipmentHandler_WhenRepositoryResultIsKnown_ShouldReturnExpectedStatusAsync(bool exists, ResultStatus expectedStatus)
    {
        var repository = new Mock<IUserQueryRepository>();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        repository.Setup(x => x.GetUserShipmentDetailsAsync(userId, shipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? ShipmentDto(shipmentId, "TN-USER") : null);
        var handler = new GetUserShipmentHandler(repository.Object, Mock.Of<ILogger<GetUserShipmentHandler>>());

        var result = await handler.Handle(new GetUserShipmentQuery(userId, shipmentId), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
        if (exists)
            result.Data!.Id.Should().Be(shipmentId);
    }

    private static RetrieveShipmentDto ShipmentDto(Guid id, string trackingNumber) => new()
    {
        Id = id,
        TrackingNumber = trackingNumber,
        DeliveryAddress = AddressDto.FromDomain(Address.Create("Cairo", null, "Main Street")),
        PickupAddress = AddressDto.FromDomain(Address.Create("Giza", null, "Main Street")),
        ShippingCost = 125,
        CurrentStatus = ShipmentStatuses.Pending
    };
}
