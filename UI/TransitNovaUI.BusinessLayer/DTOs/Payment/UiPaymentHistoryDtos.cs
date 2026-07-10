using TransitNova.Domain.Enums.Payment;

namespace TransitNovaUI.BusinessLayer.DTOs.Payment;

public sealed class UiPaymentHistoryFilterDto
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

public sealed class UiPaymentHistoryDetailsDto
{
    public int Id { get; set; }
    public Guid PaymentId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
