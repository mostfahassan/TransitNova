using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;

namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository
{
    public interface IPaymentQueryRepository
    {
        Task<PagedResult<PaymentHistoryDetailsDto>> FilterPaymentHistoryAsync(
            FilterPaymentHistoryDto filterDto,
            CancellationToken cancellationToken);
    }
}
