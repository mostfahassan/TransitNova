using TransitNovaPayment.Busieness.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Mapping
{
    public  static class PaymentMappingExtensions
    {
        public static PaymentDetailsDto MapToDetailsDto(this Payment payment,decimal commission)
        {
           ArgumentNullException.ThrowIfNull(payment);
            return new PaymentDetailsDto
            {
                PaymentId = payment.Id,
                ReferenceId = payment.ReferenceId,
                ReferenceType = payment.ReferenceType.ToString(),
                TotalAmount = payment.TotalAmount,
                PaymentMethod = payment.PaymentMethod.ToString(),
                Status = payment.Status.ToString(),
                PaidAt = payment.PaidAt,
                Notes = payment.Status == PaymentStatus.Failed
                    ? payment.FailedReason
                    : "Payment processed successfully.",
                Commission = commission
            };
        }
    }
}
