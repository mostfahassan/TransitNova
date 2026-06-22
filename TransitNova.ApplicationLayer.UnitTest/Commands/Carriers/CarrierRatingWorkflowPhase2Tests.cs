using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRatingRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Commands.Carriers;

public sealed class CarrierRatingWorkflowPhase2Tests
{
    [Fact]
    public async Task RatingPickupCarrierHandler_WhenUserCannotRate_ShouldReturnFailureWithoutSaving()
    {
        var fixture = new PickupRatingFixture();
        fixture.ShipmentRules.Setup(x => x.CanRatePickUpCarrierAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RatingPickupCarrierHandler_WhenCarrierDoesNotExist_ShouldReturnNotFoundWithoutSaving()
    {
        var fixture = new PickupRatingFixture();
        fixture.Carriers.Setup(x => x.GetCarrierAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrier?)null);
        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RatingPickupCarrierHandler_WhenRatingIsAllowed_ShouldReturnSuccess()
    {
        var fixture = new PickupRatingFixture();
        (await fixture.Handler.Handle(fixture.Command, CancellationToken.None)).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RatingPickupCarrierHandler_WhenRatingIsAllowed_ShouldPersistCorrectRatingRecord()
    {
        var fixture = new PickupRatingFixture();
        CarrierRating? captured = null;
        fixture.Ratings.Setup(x => x.AddRatingAsync(It.IsAny<CarrierRating>(), It.IsAny<CancellationToken>()))
            .Callback<CarrierRating, CancellationToken>((rating, _) => captured = rating)
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        captured!.CarrierId.Should().Be(fixture.Dto.CarrierId);
        captured.ShipmentId.Should().Be(fixture.Command.shipmentId);
        captured.CustomerId.Should().Be(fixture.Command.AppUserId);
    }

    [Fact]
    public async Task RatingPickupCarrierHandler_WhenRatingIsAllowed_ShouldSaveAndInvalidateThreeCaches()
    {
        var fixture = new PickupRatingFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RatingPickupCarrierHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new PickupRatingFixture();
        using var cancellation = new CancellationTokenSource();
        await fixture.Handler.Handle(fixture.Command, cancellation.Token);
        fixture.Ratings.Verify(x => x.AddRatingAsync(It.IsAny<CarrierRating>(), cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenUserCannotRate_ShouldReturnFailureWithoutSaving()
    {
        var fixture = new DeliveryRatingFixture();
        fixture.ShipmentRules.Setup(x => x.CanRateDeliveryCarrierAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenCarrierDoesNotExist_ShouldReturnNotFoundWithoutSaving()
    {
        var fixture = new DeliveryRatingFixture();
        fixture.Carriers.Setup(x => x.GetCarrierAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrier?)null);
        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenRatingIsAllowed_ShouldReturnSuccess()
    {
        var fixture = new DeliveryRatingFixture();
        (await fixture.Handler.Handle(fixture.Command, CancellationToken.None)).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenRatingIsAllowed_ShouldPersistRatingAndComment()
    {
        var fixture = new DeliveryRatingFixture();
        CarrierRating? captured = null;
        fixture.Ratings.Setup(x => x.AddRatingAsync(It.IsAny<CarrierRating>(), It.IsAny<CancellationToken>()))
            .Callback<CarrierRating, CancellationToken>((rating, _) => captured = rating)
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        captured!.Rating.Should().Be(fixture.Dto.Rating);
        captured.Comment.Should().Be(fixture.Dto.Comment);
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenRatingIsAllowed_ShouldSaveAndInvalidateThreeCaches()
    {
        var fixture = new DeliveryRatingFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RatingDeliveryCarrierHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new DeliveryRatingFixture();
        using var cancellation = new CancellationTokenSource();
        await fixture.Handler.Handle(fixture.Command, cancellation.Token);
        fixture.Ratings.Verify(x => x.AddRatingAsync(It.IsAny<CarrierRating>(), cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    private sealed class PickupRatingFixture
    {
        public Carrier Carrier { get; } = Carrier.Create(
            Guid.NewGuid(), "Ahmed", "Ali", "carrier@example.com", "01000000000", "Cairo", 1);
        public RatingCarrierDto Dto { get; }
        public RatePickupCarrierCommand Command { get; }
        public Mock<IShipmentRulesRepository> ShipmentRules { get; } = new();
        public Mock<ICarrierQueryRepository> Carriers { get; } = new();
        public Mock<ICarrierRatingCommandsRepository> Ratings { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public RatingPickupCarrierHandler Handler { get; }
        public PickupRatingFixture()
        {
            Dto = new RatingCarrierDto(Carrier.Id, "Careful handling", 5);
            Command = new RatePickupCarrierCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Dto);
            ShipmentRules.Setup(x => x.CanRatePickUpCarrierAsync(
                    Command.shipmentId, Carrier.Id, Command.AppUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            Carriers.Setup(x => x.GetCarrierAsync(
                    It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Carrier);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new RatingPickupCarrierHandler(
                ShipmentRules.Object, Carriers.Object, Ratings.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<RatingPickupCarrierHandler>>());
        }
    }

    private sealed class DeliveryRatingFixture
    {
        public Carrier Carrier { get; } = Carrier.Create(
            Guid.NewGuid(), "Ahmed", "Ali", "carrier@example.com", "01000000000", "Cairo", 1);
        public RatingCarrierDto Dto { get; }
        public RateDeliveryCarrierCommand Command { get; }
        public Mock<IShipmentRulesRepository> ShipmentRules { get; } = new();
        public Mock<ICarrierQueryRepository> Carriers { get; } = new();
        public Mock<ICarrierRatingCommandsRepository> Ratings { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public RatingDeliveryCarrierHandler Handler { get; }
        public DeliveryRatingFixture()
        {
            Dto = new RatingCarrierDto(Carrier.Id, "Fast delivery", 4);
            Command = new RateDeliveryCarrierCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Dto);
            ShipmentRules.Setup(x => x.CanRateDeliveryCarrierAsync(
                    Command.shipmentId, Carrier.Id, Command.AppUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            Carriers.Setup(x => x.GetCarrierAsync(
                    It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Carrier);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new RatingDeliveryCarrierHandler(
                ShipmentRules.Object, Carriers.Object, Ratings.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<RatingDeliveryCarrierHandler>>());
        }
    }
}
