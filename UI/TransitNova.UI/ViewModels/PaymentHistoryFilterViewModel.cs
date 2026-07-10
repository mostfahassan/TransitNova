using TransitNova.Domain.Enums.Payment;
using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNova.UI.ViewModels;

public sealed class PaymentHistoryFilterViewModel
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

    public UiPaymentHistoryFilterDto ToDto() => new()
    {
        PaymentId = PaymentId,
        PaymentStatus = PaymentStatus,
        PaymentMethod = PaymentMethod,
        CreatedAt = CreatedAt,
        CreatedAtFrom = CreatedAtFrom,
        CreatedAtTo = CreatedAtTo,
        CreatedBy = CreatedBy,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}
