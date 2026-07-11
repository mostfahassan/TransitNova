using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.DTO.PaymentDto
{
    public sealed class CreatePaymentDto
    {
        public Guid ReferenceId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public Currency Currency { get; init; }
        public decimal Cost { get; init; }
    }
}
