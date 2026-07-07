namespace TransitNovaUI.BusinessLayer.DTOs.Payment;

public sealed class UiInvoiceDto
{
    public Guid PaymentId { get; set; }
    public Guid ShipmentId { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Commission { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
}