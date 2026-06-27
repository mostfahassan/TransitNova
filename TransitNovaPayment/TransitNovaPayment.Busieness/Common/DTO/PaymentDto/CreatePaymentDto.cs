
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.DTO.PaymentDto
{
    public sealed class CreatePaymentDto
    {
        public Guid ShipmentId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public decimal ShippingCost { get; init; }
    }
}
