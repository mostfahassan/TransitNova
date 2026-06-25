
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.DTO.PaymentDto
{
    public sealed class CreatePaymentDto
    {
        public Guid ShipmentId { get;}
        public PaymentMethod PaymentMethod { get;}
        public decimal ShippingCost {get;}
    }
}
