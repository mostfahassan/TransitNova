using TransitNovaPayment.Busieness.Common.Contracts.Keys;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Interfaces.Common;
namespace TransitNovaPayment.Busieness.Services.Payment.Query
{
    public sealed record FilterPaymentsQuery(FilterPaymentHistoryDto Filter)
        : IQuery<PagedResult<PaymentHistoryDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.PaymentHistoryFilter(Filter);
    }
}