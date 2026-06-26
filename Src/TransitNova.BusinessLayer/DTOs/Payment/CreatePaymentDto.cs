using TransitNova.Domain.Enums.Payment;

namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public sealed record CreatePaymentDto
    {
        public Guid ShipmentId { get; }
        public PaymentMethod PaymentMethod { get; }
        public decimal ShippingCost { get; }
    }

  
    
}
