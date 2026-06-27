using TransitNova.Domain.Enums.Payment;

namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public sealed record CreatePaymentDto
    {
        public Guid ShipmentId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public decimal ShippingCost { get; init; }
    }

  
    
}
