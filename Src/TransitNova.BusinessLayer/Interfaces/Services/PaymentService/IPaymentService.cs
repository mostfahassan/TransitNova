using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Interfaces.Services.PaymentService
{
    public interface IPaymentService
    {
        Task <Result<InvoiceDto>> Pay (CreatePaymentDto createPaymentDto,CancellationToken cancellationToken);
        Task<Result<InvoiceDto>> PayForBundle(CreatePaymentDto createPaymentDto, CancellationToken cancellationToken);

    }
}
