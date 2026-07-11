using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
namespace TransitNovaPayment.Busieness.Interfaces.PaymentService
{
    public interface IShipmentPayment
    {
        Task<BaseResult?> Pay(CreatePaymentDto dto,string publicKey ,CancellationToken cancellationToken);
    }
}
