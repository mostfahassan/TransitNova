using FluentAssertions;
using Moq;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Common.Contracts.Keys;
using TransitNovaPayment.Busieness.Interfaces.Common;
using TransitNovaPayment.Busieness.Interfaces.PaymentExecution;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Busieness.Repositories.PaymentRepository;
using PaymentEntity = TransitNovaPayment.Busieness.Models.PaymentEntity.Payment;
using PaymentProcessSut = TransitNovaPayment.Busieness.Implementation.PaymentService.BundlePaymentProcess;

namespace TransitNova.Payment.Tests.PaymentProcess;

public sealed class BundlePaymentProcessTests
{
    [Fact]
    public async Task Pay_WhenGatewaySucceeds_ShouldPersistBundleReferenceAndReturnUnifiedDetailsAsync()
    {
        var fixture = new Fixture();
        var bundleId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(true));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ReturnsAsync(1);

        var result = await fixture.CreateSut().Pay(
            PaymentTestData.CreatePaymentDto(paymentMethod: PaymentMethod.PayPal, shippingCost: 200m, shipmentId: bundleId),
            "payment-private-key",
            CancellationToken.None);

        result!.IsSuccess.Should().BeTrue();
        result.Data!.ReferenceId.Should().Be(bundleId);
        result.Data.ReferenceType.Should().Be(nameof(ReferenceType.Bundle));
        result.Data.PaymentMethod.Should().Be(nameof(PaymentMethod.PayPal));
        result.Data.Status.Should().Be(nameof(PaymentStatus.Success));
        result.Data.TotalAmount.Should().BeGreaterThan(200m);
        fixture.CapturedPayment.Should().NotBeNull();
        fixture.CapturedPayment!.ReferenceId.Should().Be(bundleId);
        fixture.CapturedPayment.ReferenceType.Should().Be(ReferenceType.Bundle);
        fixture.CapturedPayment.PaymentMethod.Should().Be(PaymentMethod.PayPal);
        fixture.CallOrder.Should().Equal("persist", "save", "cache");
        fixture.Cache.Verify(x => x.RemoveByPrefixAsync(CacheKeys.PaymentsPrefix, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Pay_WhenGatewaySimulationFails_ShouldPersistFailedBundlePaymentAsync()
    {
        var fixture = new Fixture();
        var bundleId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(false, "bundle gateway timeout"));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ReturnsAsync(1);

        var result = await fixture.CreateSut().Pay(
            PaymentTestData.CreatePaymentDto(paymentMethod: PaymentMethod.CreditCard, shippingCost: 150m, shipmentId: bundleId),
            "payment-private-key",
            CancellationToken.None);

        result!.IsSuccess.Should().BeTrue();
        result.Data!.ReferenceId.Should().Be(bundleId);
        result.Data.ReferenceType.Should().Be(nameof(ReferenceType.Bundle));
        result.Data.Status.Should().Be(nameof(PaymentStatus.Failed));
        result.Data.Notes.Should().Be("bundle gateway timeout");
        fixture.CapturedPayment.Should().NotBeNull();
        fixture.CapturedPayment!.ReferenceType.Should().Be(ReferenceType.Bundle);
        fixture.CapturedPayment.Status.Should().Be(PaymentStatus.Failed);
        fixture.CallOrder.Should().Equal("persist", "save", "cache");
    }

    private sealed class Fixture
    {
        public List<string> CallOrder { get; } = [];
        public Mock<IPaymentCommandRepository> Repository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IPaymentExecutionStrategy> ExecutionStrategy { get; } = new();
        public PaymentEntity? CapturedPayment { get; private set; }

        public PaymentProcessSut CreateSut()
        {
            Repository.Setup(x => x.CreatePaymentAsync(It.IsAny<PaymentEntity>(), It.IsAny<CancellationToken>()))
                .Callback<PaymentEntity, CancellationToken>((payment, _) =>
                {
                    CapturedPayment = payment;
                    CallOrder.Add("persist");
                })
                .Returns(Task.CompletedTask);
            Cache.Setup(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback(() => CallOrder.Add("cache"))
                .Returns(Task.CompletedTask);

            return new PaymentProcessSut(
                [
                    PaymentTestData.FixedPaymentMethod(PaymentMethod.CreditCard, 0.025m),
                    PaymentTestData.FixedPaymentMethod(PaymentMethod.PayPal, 0.045m),
                    PaymentTestData.FixedPaymentMethod(PaymentMethod.MobileWallets, 0.015m)
                ],
                Repository.Object,
                UnitOfWork.Object,
                Cache.Object,
                ExecutionStrategy.Object,
                PaymentTestData.CreatePaymentGatewayOptions(),
                Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentProcessSut>.Instance);
        }
    }
}
