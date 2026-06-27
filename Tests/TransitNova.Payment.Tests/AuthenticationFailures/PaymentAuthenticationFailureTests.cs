using FluentAssertions;
using Moq;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentExecution;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern.Enums;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using PaymentProcessSut = TransitNovaPayment.Busieness.Common.Implementation.PaymentProcess;

namespace TransitNova.Payment.Tests.AuthenticationFailures;

public sealed class PaymentAuthenticationFailureTests
{
    [Fact]
    public async Task Pay_WhenPublicKeyIsInvalid_ShouldReturnUnauthorizedWithoutPersistingAsync()
    {
        var repository = new Mock<IPaymentCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var executionStrategy = new Mock<IPaymentExecutionStrategy>();
        var sut = CreateSut(repository, unitOfWork, cache, executionStrategy, PaymentTestData.CreatePaymentGatewayOptions());

        var result = await sut.Pay(PaymentTestData.CreatePaymentDto(paymentMethod: PaymentMethod.PayPal), "invalid-key", CancellationToken.None);

        result!.Status.Should().Be(ResultStatus.Unauthorized);
        repository.Verify(x => x.CreatePaymentAsync(It.IsAny<TransitNovaPayment.Busieness.Models.PaymentEntity.Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Pay_WhenPrivateKeyIsMissing_ShouldThrowAndSkipPersistenceAsync()
    {
        var repository = new Mock<IPaymentCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var executionStrategy = new Mock<IPaymentExecutionStrategy>();
        var sut = CreateSut(repository, unitOfWork, cache, executionStrategy, PaymentTestData.CreatePaymentGatewayOptions(privateKey: null));

        var act = () => sut.Pay(PaymentTestData.CreatePaymentDto(), "payment-private-key", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        repository.Verify(x => x.CreatePaymentAsync(It.IsAny<TransitNovaPayment.Busieness.Models.PaymentEntity.Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static PaymentProcessSut CreateSut(
        Mock<IPaymentCommandRepository> repository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<ICacheService> cache,
        Mock<IPaymentExecutionStrategy> executionStrategy,
        Microsoft.Extensions.Options.IOptions<TransitNovaPayment.Busieness.Common.Options.PaymentGatewaySettings> paymentGatewayOptions)
    {
        return new PaymentProcessSut(
            [
                PaymentTestData.FixedPaymentMethod(PaymentMethod.CreditCard, 0.025m),
                PaymentTestData.FixedPaymentMethod(PaymentMethod.PayPal, 0.045m)
            ],
            repository.Object,
            unitOfWork.Object,
            cache.Object,
            executionStrategy.Object,
            paymentGatewayOptions,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentProcessSut>.Instance);
    }
}
