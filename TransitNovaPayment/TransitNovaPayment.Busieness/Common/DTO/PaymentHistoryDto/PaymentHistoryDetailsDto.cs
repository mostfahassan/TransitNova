namespace TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto
{
    public sealed class PaymentHistoryDetailsDto
    {
        public int Id { get; init; }
        public Guid PaymentId { get; init; }
        public string OldStatus { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
        public DateTime ChangedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? CreatedBy { get; init; }
    }
}
