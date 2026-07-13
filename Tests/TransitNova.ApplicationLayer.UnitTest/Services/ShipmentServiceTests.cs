using FluentAssertions;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Services.ShipmentServices;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class ShipmentServiceTests
{
    [Fact]
    public async Task HandleShipmentCreation_Should_PersistAggregateAndInvoice_WhenPaymentSucceedsAsync()
    {
        var fixture = new Fixture();
        var subscriptionId = Guid.NewGuid();
        var bundleId = Guid.NewGuid();
        fixture.BundleBenefit.Setup(x => x.CalculateShipmentBenefitAsync(
                fixture.ProfileId,
                100m,
                It.IsAny<PackageSpecificationDto>(),
                CancellationToken.None))
            .ReturnsAsync(new BundleBenefitResultDto
            {
                BundleSubscriptionId = subscriptionId,
                BundleId = bundleId,
                BundleName = "Starter",
                OriginalShippingCost = 100m,
                DiscountPercentage = 10m,
                DiscountAmount = 10m,
                FinalShippingCost = 90m,
                SubscriptionBenefitApplied = true,
                SubscriptionBenefitMessage = "10% discount applied."
            });
        CreatePaymentDto? sentPayment = null;
        fixture.PaymentService.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), CancellationToken.None))
            .Callback<CreatePaymentDto, CancellationToken>((dto, _) => sentPayment = dto)
            .ReturnsAsync(Result<InvoiceDto>.Success(ValidInvoice(90m)));
        Shipment? storedShipment = null;
        PaymentInvoice? storedInvoice = null;
        fixture.ShipmentCommand.Setup(x => x.AddAsync(It.IsAny<Shipment>(), CancellationToken.None))
            .Callback<Shipment, CancellationToken>((shipment, _) => storedShipment = shipment)
            .Returns(Task.CompletedTask);
        fixture.PaymentCommand.Setup(x => x.CreateInvoice(It.IsAny<PaymentInvoice>(), CancellationToken.None))
            .Callback<PaymentInvoice, CancellationToken>((invoice, _) => storedInvoice = invoice)
            .Returns(Task.CompletedTask);

        var (result, trackingNumber) = await fixture.CreateSut().HandleShipmentCreation(
            ValidCreateDto(),
            fixture.AppUserId,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.SubscriptionBenefitApplied.Should().BeTrue();
        result.Data.DiscountAmount.Should().Be(10m);
        result.Data.FinalShippingCost.Should().Be(90m);
        trackingNumber.Should().NotBeNullOrWhiteSpace();
        sentPayment!.Cost.Should().Be(90m);
        storedShipment.Should().NotBeNull();
        storedInvoice.Should().NotBeNull();
        fixture.Receiver.Verify(x => x.CreateReceiverAsync(It.IsAny<ReceiverProfile>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleShipmentCreation_Should_ReturnFailureWithoutPersistence_WhenPaymentResponseIsNullAsync()
    {
        var fixture = new Fixture();
        fixture.PaymentService.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Result<InvoiceDto>)null!);

        var (result, trackingNumber) = await fixture.CreateSut().HandleShipmentCreation(
            ValidCreateDto(), fixture.AppUserId, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Contain("Payment operation failed");
        trackingNumber.Should().BeEmpty();
        fixture.VerifyNothingPersisted();
    }

    [Fact]
    public async Task HandleShipmentCreation_Should_PreservePaymentFailureReasonAsync()
    {
        var fixture = new Fixture();
        fixture.PaymentService.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure(Errors.FailedOperation("Transaction declined.")));

        var (result, _) = await fixture.CreateSut().HandleShipmentCreation(
            ValidCreateDto(), fixture.AppUserId, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Transaction declined.");
        fixture.VerifyNothingPersisted();
    }

    [Theory]
    [InlineData("invalid-method", "Success")]
    [InlineData("CreditCard", "invalid-status")]
    public async Task HandleShipmentCreation_Should_RejectInvalidPaymentEnumValuesAsync(string method, string status)
    {
        var fixture = new Fixture();
        fixture.PaymentService.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(ValidInvoice(100m, method, status)));

        var (result, _) = await fixture.CreateSut().HandleShipmentCreation(
            ValidCreateDto(), fixture.AppUserId, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Contain("invalid payment values");
        fixture.VerifyNothingPersisted();
    }

    [Fact]
    public void UpdateShipmentDetails_Should_Recalculate_WhenPricingInputsChange()
    {
        var fixture = new Fixture();
        var shipment = ShipmentTestData.CreateShipment();
        fixture.Pricing.Reset();
        fixture.Pricing.Setup(x => x.CalculateShipment(
                It.IsAny<PackageSpecification>(),
                enShipmentType.Express,
                TransportationMode.Air))
            .Returns((250m, DateTime.UtcNow.AddDays(2)));
        var update = new UpdateShipmentDto(
            null,
            Address("Updated delivery"),
            Address("Updated pickup"),
            new PackageSpecificationDto { Weight = 5, Width = 10, Height = 10, Length = 10 },
            enShipmentType.Express,
            TransportationMode.Air);

        fixture.CreateSut().UpdateShipmentDetails(shipment, update);

        shipment.Mode.Should().Be(TransportationMode.Air);
        shipment.ShipmentType.Should().Be(enShipmentType.Express);
        fixture.Pricing.VerifyAll();
    }

    [Fact]
    public void UpdateShipmentDetails_Should_NotRecalculate_WhenPricingInputsAreUnchanged()
    {
        var fixture = new Fixture();
        var shipment = ShipmentTestData.CreateShipment();
        fixture.Pricing.Reset();

        fixture.CreateSut().UpdateShipmentDetails(
            shipment,
            new UpdateShipmentDto(null, null, null, null, null, null));

        fixture.Pricing.Verify(x => x.CalculateShipment(
            It.IsAny<PackageSpecification>(),
            It.IsAny<enShipmentType>(),
            It.IsAny<TransportationMode>()), Times.Never);
    }

    private static CreateShipmentDto ValidCreateDto() => new(
        new CreateReceiverDto
        {
            FirstName = "Mona",
            LastName = "Ali",
            Email = "mona@example.com",
            PhoneNumber = "+201001234567",
            Address = Address("Receiver address"),
            CityId = 1
        },
        new PackageSpecificationDto { Weight = 2, Width = 10, Height = 10, Length = 10 },
        Currency.EGP,
        DateTime.UtcNow.AddDays(1),
        TransportationMode.Land,
        enShipmentType.Standard,
        Address("Delivery address"),
        Address("Pickup address"),
        Guid.Empty,
        PaymentMethod.CreditCard);

    private static AddressDto Address(string main) => new()
    {
        MainAddress = main,
        Street = "Main Street"
    };

    private static InvoiceDto ValidInvoice(
        decimal amount,
        string method = "CreditCard",
        string status = "Success") => new()
    {
        PaymentId = Guid.NewGuid(),
        ReferenceId = Guid.NewGuid(),
        ReferenceType = "Shipment",
        Amount = amount,
        Commission = 2.5m,
        TotalAmount = amount + 2.5m,
        PaymentMethod = method,
        Status = status,
        PaidAt = DateTime.UtcNow,
        Notes = "Approved"
    };

    private sealed class Fixture
    {
        internal Guid AppUserId { get; } = Guid.NewGuid();
        internal Guid ProfileId { get; } = Guid.NewGuid();
        internal Mock<IShipmentCommandRepository> ShipmentCommand { get; } = new();
        internal Mock<IPaymentRepositoryCommand> PaymentCommand { get; } = new();
        internal Mock<IReceiverRepository> Receiver { get; } = new();
        internal Mock<IPaymentService> PaymentService { get; } = new();
        internal Mock<IUserQueryRepository> User { get; } = new();
        internal Mock<IShipmentPricingServices> Pricing { get; } = new();
        internal Mock<IBundleBenefitService> BundleBenefit { get; } = new();

        internal Fixture()
        {
            User.Setup(x => x.GetAppUserIdAsync(AppUserId, It.IsAny<CancellationToken>())).ReturnsAsync(ProfileId);
            Pricing.Setup(x => x.CalculateShipment(It.IsAny<PackageSpecification>(), It.IsAny<enShipmentType>(), It.IsAny<TransportationMode>()))
                .Returns((100m, DateTime.UtcNow.AddDays(3)));
            BundleBenefit.Setup(x => x.CalculateShipmentBenefitAsync(ProfileId, 100m, It.IsAny<PackageSpecificationDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BundleBenefitResultDto.None(100m));
        }

        internal ShipmentService CreateSut() => new(
            ShipmentCommand.Object,
            PaymentCommand.Object,
            Receiver.Object,
            PaymentService.Object,
            User.Object,
            Pricing.Object,
            BundleBenefit.Object);

        internal void VerifyNothingPersisted()
        {
            Receiver.Verify(x => x.CreateReceiverAsync(It.IsAny<ReceiverProfile>(), It.IsAny<CancellationToken>()), Times.Never);
            ShipmentCommand.Verify(x => x.AddAsync(It.IsAny<Shipment>(), It.IsAny<CancellationToken>()), Times.Never);
            PaymentCommand.Verify(x => x.CreateInvoice(It.IsAny<PaymentInvoice>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

