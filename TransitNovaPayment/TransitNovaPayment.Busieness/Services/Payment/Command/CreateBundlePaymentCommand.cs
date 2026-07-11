using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
namespace TransitNovaPayment.Busieness.Services.Payment.Command
{
    public sealed record CreateBundlePaymentCommand(CreatePaymentDto Dto, string Key) : ICommand<BaseResult>;
}
