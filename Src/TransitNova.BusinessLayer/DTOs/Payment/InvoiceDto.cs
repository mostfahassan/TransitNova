using TransitNova.Domain.Enums.Shipment;
using static TransitNova.Domain.Contracts.Constants.Constant;
namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public class InvoiceDto
    {
        public Guid PaymentId { get; init; }
        public Guid ReferenceId { get; init; }
        public string? ReferenceType { get; init; }
        public decimal Amount { get; init; }
        public decimal Commission { get; init; }
        public decimal TotalAmount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime? PaidAt { get; init; }
        public string? Notes { get; init; }
    }

    public class ShipmentPaymentInvoiceDto
    {
        public string? InvoiceId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid ReferenceId { get; set; }
        public string ReferenceType { get; set; } = PaymentReferenceConstants.Shipment;
        public Guid ShipmentId { get; set; }
        public string? ShipmentTrackingNumber { get; set; }
        public string? CustomerName { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; init; }
        public Currency Currency { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class BundlePaymentInvoiceDto
    {
        public string? InvoiceId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid ReferenceId { get; set; }
        public string ReferenceType { get; set; } = PaymentReferenceConstants.Bundle;
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
}
