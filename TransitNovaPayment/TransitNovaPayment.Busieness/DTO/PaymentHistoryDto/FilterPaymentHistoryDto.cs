using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.DTO.PaymentHistoryDto
{
    public sealed class FilterPaymentHistoryDto
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
}