using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.Admin.Handler;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;

namespace TransitNova.ApplicationLayer.Tests.Queries;

public sealed class AdminPaymentHistoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnSamePagedResultFromServiceAsync()
    {
        var filter = new PaymentHistoryFilterDto { PageNumber = 2, PageSize = 5 };
        var pagedResult = PagedResult<PaymentHistoryDetailsDto>.From(
            [new PaymentHistoryDetailsDto { Id = 7, PaymentId = Guid.NewGuid(), OldStatus = "Pending", NewStatus = "Success" }],
            1,
            2,
            5);
        var service = new Mock<IPaymentHistoryService>();
        service.Setup(x => x.GetPaymentHistoriesAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<PaymentHistoryDetailsDto>>.Success(pagedResult));
        var handler = new GetAdminPaymentHistoriesHandler(
            service.Object,
            Mock.Of<ILogger<GetAdminPaymentHistoriesHandler>>());

        var result = await handler.Handle(new GetAdminPaymentHistoriesQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(pagedResult);
        service.Verify(x => x.GetPaymentHistoriesAsync(filter, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_PropagateServiceFailureAsync()
    {
        var filter = new PaymentHistoryFilterDto();
        var expected = Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
            Errors.FailedOperation("Payment service unreachable"));
        var service = new Mock<IPaymentHistoryService>();
        service.Setup(x => x.GetPaymentHistoriesAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var handler = new GetAdminPaymentHistoriesHandler(
            service.Object,
            Mock.Of<ILogger<GetAdminPaymentHistoriesHandler>>());

        var result = await handler.Handle(new GetAdminPaymentHistoriesQuery(filter), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment service unreachable");
    }
}
