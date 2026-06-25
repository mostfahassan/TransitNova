using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentService
{
    public interface IPayment
    {
        Task<BaseResult?> Pay(CreatePaymentDto dto,CancellationToken cancellationToken);
    }
}
