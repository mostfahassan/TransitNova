
namespace TransitNovaPayment.Busieness.Common.DTO.PaymentDto
{
    public sealed class PaymentDetailsDto
    {
        public Guid PaymentId { get; init; }
        public Guid ShipmentId { get; init; }
        public decimal Amount { get; init; }
        public decimal Commission { get; init; }
        public decimal TotalAmount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime? PaidAt { get; init; }
        public string? Notes { get; init; }

    }
}
