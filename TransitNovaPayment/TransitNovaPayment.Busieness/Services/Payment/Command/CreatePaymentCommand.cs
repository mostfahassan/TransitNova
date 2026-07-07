
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
namespace TransitNovaPayment.Busieness.Services.Payment.Command
{
    public sealed record CreatePaymentCommand(CreatePaymentDto Dto, string Key) : ICommand<BaseResult>;
   
}
