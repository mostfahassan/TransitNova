using System;
using System.Collections.Generic;
using System.Text;

namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public sealed class PaymentGatewayResponse
    {
        public PaymentGatewayInvoice? Data { get; init; }
        public bool IsSuccess { get; init; }
        public bool IsFailure { get; init; }
        public string? Message { get; init; }
        public int StatusCode { get; init; }
        public PaymentGatewayError? Error { get; init; }
    }

    public sealed class PaymentGatewayInvoice
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

        public Invoice ToInvoice(decimal shippingCost)
        {
            return new Invoice
            {
                PaymentId = PaymentId,
                ShipmentId = ShipmentId,
                ShippingCost = shippingCost,
                Commission = Commission,
                Amount = TotalAmount,
                PaymentMethod = PaymentMethod,
                Status = Status,
                PaidAt = PaidAt,
                Notes = Notes
            };
        }
    }

    public sealed class PaymentGatewayError
    {
        public string? Message { get; init; }
    }
}
