using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Payment;

public sealed class UiInvoiceDto
{
    public string? InvoiceId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = "Shipment";
    public Guid ShipmentId { get; set; }
    public string? ShipmentTrackingNumber { get; set; }
    public string? CustomerName { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Commission { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public Currency Currency { get; set; }
    public string? Notes { get; set; }
}

public sealed class UiBundleInvoiceDto
{
    public string? InvoiceId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = "Bundle";
    public Guid BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal BundlePrice { get; set; }
    public decimal Commission { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SubscribedAt { get; set; }
    public string? Notes { get; set; }
}
