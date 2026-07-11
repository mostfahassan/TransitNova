using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public sealed record CreatePaymentDto
    {
        public Guid ReferenceId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public Currency Currency { get; init; }
        public decimal Cost { get; init; }
    }

}
