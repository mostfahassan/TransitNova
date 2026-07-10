using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Payment;

public sealed class UiInvoiceDto
{
    public string? InvoiceId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid ShipmentId { get; set; }
    public string? ShipmentTrackingNumber { get; set; }
    public string? CustomerName { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Commission { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public Currency Currency { get; set; }
    public string? Notes { get; set; }
}
