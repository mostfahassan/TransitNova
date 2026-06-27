using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.API.Controllers.Payment;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Services.Payment.Command;
using TransitNovaPayment.Busieness.Services.Payment.Query;

namespace TransitNova.Payment.Tests.Handlers;

public sealed class PaymentControllerTests
{
    [Fact]
    public async Task Pay_ShouldSendCommandAndReturnStatusCodeFromResultAsync()
    {
        var mediator = new Mock<IMediator>();
        var dto = PaymentTestData.CreatePaymentDto();
        var expected = BaseResult.Success(new TransitNovaPayment.Busieness.Common.DTO.PaymentDto.PaymentDetailsDto());
        mediator.Setup(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);
        var controller = new PaymentController(mediator.Object);

        var result = await controller.Pay("payment-private-key", dto, CancellationToken.None);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task History_ShouldSendQueryAndReturnOkResultAsync()
    {
        var mediator = new Mock<IMediator>();
        var dto = new FilterPaymentHistoryDto { PageNumber = 1, PageSize = 5 };
        var expected = PagedResult<PaymentHistoryDetailsDto>.Page([], 0, 1, 5);
        mediator.Setup(x => x.Send(It.IsAny<FilterPaymentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var controller = new PaymentController(mediator.Object);

        var result = await controller.History(dto, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(expected);
    }
}
