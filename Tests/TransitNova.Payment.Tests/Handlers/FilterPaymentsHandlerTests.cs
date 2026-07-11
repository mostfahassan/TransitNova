using FluentAssertions;
using Moq;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Services.Payment.Query;
using TransitNovaPayment.Busieness.Services.Payment.Handler.QueriesHandler;

namespace TransitNova.Payment.Tests.Handlers;

public sealed class FilterPaymentsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDelegateToRepositoryAndReturnPagedResultAsync()
    {
        var expected = PagedResult<PaymentHistoryDetailsDto>.Page(
            [new PaymentHistoryDetailsDto { PaymentId = Guid.Parse("44444444-4444-4444-4444-444444444444") }],
            1,
            2,
            5);
        var repository = new Mock<IPaymentQueryRepository>();
        var filter = new FilterPaymentHistoryDto { PageNumber = 2, PageSize = 5 };
        repository.Setup(x => x.FilterPaymentHistoryAsync(filter, It.IsAny<CancellationToken>())).ReturnsAsync(expected);
        var handler = new FilterPaymentsHandler(repository.Object, Microsoft.Extensions.Logging.Abstractions.NullLogger<FilterPaymentsHandler>.Instance);

        var result = await handler.Handle(new FilterPaymentsQuery(filter), CancellationToken.None);

        result.Should().BeSameAs(expected);
        repository.Verify(x => x.FilterPaymentHistoryAsync(filter, CancellationToken.None), Times.Once);
    }
}

