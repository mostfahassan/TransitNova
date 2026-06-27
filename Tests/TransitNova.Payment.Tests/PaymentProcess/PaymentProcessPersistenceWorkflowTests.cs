using FluentAssertions;
using Moq;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentExecution;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Contracts.Keys;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using PaymentProcessSut = TransitNovaPayment.Busieness.Common.Implementation.PaymentProcess;

namespace TransitNova.Payment.Tests.PaymentProcess;

public sealed class PaymentProcessPersistenceWorkflowTests
{
    [Fact]
    public async Task Pay_WhenSaveSucceeds_ShouldInvalidateCacheAfterPersistenceAsync()
    {
        var fixture = new Fixture();
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(true));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ReturnsAsync(1);

        var result = await fixture.CreateSut().Pay(
            PaymentTestData.CreatePaymentDto(paymentMethod: PaymentMethod.PayPal),
            "payment-private-key",
            CancellationToken.None);

        result!.IsSuccess.Should().BeTrue();
        fixture.CallOrder.Should().Equal("persist", "save", "cache");
        fixture.Cache.Verify(x => x.RemoveByPrefixAsync(CacheKeys.PaymentsPrefix, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Pay_WhenSaveReturnsZero_ShouldNotInvalidateCacheAsync()
    {
        var fixture = new Fixture();
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(true));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ReturnsAsync(0);

        var result = await fixture.CreateSut().Pay(PaymentTestData.CreatePaymentDto(), "payment-private-key", CancellationToken.None);

        result!.IsFailure.Should().BeTrue();
        fixture.CallOrder.Should().Equal("persist", "save");
        fixture.Cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Pay_WhenSaveThrows_ShouldNotInvalidateCacheAsync()
    {
        var fixture = new Fixture();
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(true));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));

        var act = () => fixture.CreateSut().Pay(PaymentTestData.CreatePaymentDto(), "payment-private-key", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
        fixture.CallOrder.Should().Equal("persist", "save");
        fixture.Cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Pay_WhenGatewaySimulationFails_ShouldPersistFailureThenInvalidateCacheAsync()
    {
        var fixture = new Fixture();
        fixture.ExecutionStrategy.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentExecutionResult(false, "gateway timeout"));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => fixture.CallOrder.Add("save"))
            .ReturnsAsync(1);

        var result = await fixture.CreateSut().Pay(PaymentTestData.CreatePaymentDto(), "payment-private-key", CancellationToken.None);

        result!.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(nameof(PaymentStatus.Failed));
        result.Data.Notes.Should().Be("gateway timeout");
        fixture.CallOrder.Should().Equal("persist", "save", "cache");
        fixture.Cache.Verify(x => x.RemoveByPrefixAsync(CacheKeys.PaymentsPrefix, CancellationToken.None), Times.Once);
    }

    private sealed class Fixture
    {
        public List<string> CallOrder { get; } = [];
        public Mock<IPaymentCommandRepository> Repository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IPaymentExecutionStrategy> ExecutionStrategy { get; } = new();

        public PaymentProcessSut CreateSut()
        {
            Repository.Setup(x => x.CreatePaymentAsync(It.IsAny<TransitNovaPayment.Busieness.Models.PaymentEntity.Payment>(), It.IsAny<CancellationToken>()))
                .Callback(() => CallOrder.Add("persist"))
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
                PaymentTestData.CreateConfiguration(),
                Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentProcessSut>.Instance);
        }
    }
}
