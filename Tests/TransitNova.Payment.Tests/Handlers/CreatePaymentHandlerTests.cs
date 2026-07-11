using FluentAssertions;
using Moq;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Interfaces.PaymentService;
using TransitNovaPayment.Busieness.Services.Payment.Handler.CommandsHandler;
using TransitNovaPayment.Busieness.Services.Payment.Command;

namespace TransitNova.Payment.Tests.Handlers;

public sealed class CreatePaymentHandlerTests
{
    [Fact]
    public async Task Handle_WhenPaymentReturnsNull_ShouldReturnFailureResultAsync()
    {
        var payment = new Mock<IShipmentPayment>();
        payment.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BaseResult?)null);
        var handler = new CreateShipmentPaymentHandler(payment.Object, Microsoft.Extensions.Logging.Abstractions.NullLogger<CreateShipmentPaymentHandler>.Instance);

        var result = await handler.Handle(new CreateShipmentPaymentCommand(PaymentTestData.CreatePaymentDto(), "key"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Failure("Payment process returned an unexpected null result."));
    }

    [Fact]
    public async Task Handle_WhenPaymentFails_ShouldReturnOriginalResultAsync()
    {
        var expected = BaseResult.Unauthorized(Errors.UnAuthorized("Invalid payment gateway authentication key."));
        var payment = new Mock<IShipmentPayment>();
        payment.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var handler = new CreateShipmentPaymentHandler(payment.Object, Microsoft.Extensions.Logging.Abstractions.NullLogger<CreateShipmentPaymentHandler>.Instance);

        var result = await handler.Handle(new CreateShipmentPaymentCommand(PaymentTestData.CreatePaymentDto(), "bad-key"), CancellationToken.None);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_WhenPaymentSucceeds_ShouldReturnOriginalSuccessAsync()
    {
        var expected = BaseResult.Success(new PaymentDetailsDto { PaymentId = Guid.Parse("33333333-3333-3333-3333-333333333333") });
        var payment = new Mock<IShipmentPayment>();
        payment.Setup(x => x.Pay(It.IsAny<CreatePaymentDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var handler = new CreateShipmentPaymentHandler(payment.Object, Microsoft.Extensions.Logging.Abstractions.NullLogger<CreateShipmentPaymentHandler>.Instance);

        var result = await handler.Handle(new CreateShipmentPaymentCommand(PaymentTestData.CreatePaymentDto(), "key"), CancellationToken.None);

        result.Should().BeSameAs(expected);
    }
}

