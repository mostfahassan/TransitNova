using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Features.Payment.Command
{
    public sealed record CreatePaymentCommand(CreatePaymentDto Dto,Guid IdempotentKey) 
        : IdempotentCommand<Result<Invoice>>(IdempotentKey);
   
}
