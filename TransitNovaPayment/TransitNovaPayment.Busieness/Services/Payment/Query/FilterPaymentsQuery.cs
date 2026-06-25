using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.Contracts.Keys;
namespace TransitNovaPayment.Busieness.Services.Payment.Query
{
    public sealed record FilterPaymentsQuery(FilterPaymentHistoryDto Filter)
        : IQuery<PagedResult<PaymentHistoryDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.PaymentHistoryFilter(Filter);
    }
}