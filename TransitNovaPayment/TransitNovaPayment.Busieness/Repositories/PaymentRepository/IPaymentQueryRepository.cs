using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.DTO.PaymentHistoryDto;

namespace TransitNovaPayment.Busieness.Repositories.PaymentRepository
{
    public interface IPaymentQueryRepository
    {
        Task<PagedResult<PaymentHistoryDetailsDto>> FilterPaymentHistoryAsync(
            FilterPaymentHistoryDto filterDto,
            CancellationToken cancellationToken);
    }
}
