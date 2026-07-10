using TransitNova.Domain.Enums.Payment;

namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public sealed class PaymentHistoryFilterDto
    {
        public Guid? PaymentId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string? CreatedBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public sealed class PaymentHistoryDetailsDto
    {
        public int Id { get; init; }
        public Guid PaymentId { get; init; }
        public PaymentMethod? PaymentMethod { get; init; }
        public string OldStatus { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
        public DateTime ChangedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? CreatedBy { get; init; }
    }
}
