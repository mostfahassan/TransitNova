using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Interfaces.Services.PaymentService
{
    public interface IPaymentHistoryService
    {
        Task<Result<PagedResult<PaymentHistoryDetailsDto>>> GetPaymentHistoriesAsync(PaymentHistoryFilterDto filter, CancellationToken cancellationToken);


    }
}
