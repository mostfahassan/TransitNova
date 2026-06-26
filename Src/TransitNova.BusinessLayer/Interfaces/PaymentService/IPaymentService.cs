using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Interfaces.PaymentService
{
    public interface IPaymentService
    {
        Task <Result<Invoice>> Pay (CreatePaymentDto createPaymentDto,CancellationToken cancellationToken);
    }
}
