using Microsoft.Extensions.Logging;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.Services.Payment.Query;

namespace TransitNovaPayment.Busieness.Services.Payment.Handler.QueriesHandler
{
    internal sealed class FilterPaymentsHandler(
        IPaymentQueryRepository paymentQueryRepository,
        ILogger<FilterPaymentsHandler> logger)
        : IQueryHandler<FilterPaymentsQuery, PagedResult<PaymentHistoryDetailsDto>>
    {
        public async Task<PagedResult<PaymentHistoryDetailsDto>> Handle(
            FilterPaymentsQuery request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Filtering payment histories. PageNumber: {PageNumber}, PageSize: {PageSize}.",
                request.Filter.PageNumber,
                request.Filter.PageSize);

            var paymentHistories = await paymentQueryRepository.FilterPaymentHistoryAsync(
                request.Filter,
                cancellationToken);

            logger.LogInformation(
                "Payment histories filtered successfully. TotalCount: {TotalCount}.",
                paymentHistories.TotalCount);

            return paymentHistories;
        }
    }
}