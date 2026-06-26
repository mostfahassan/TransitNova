namespace TransitNova.BusinessLayer.DTOs.Payment
{
    public class Invoice
    {
        public Guid PaymentId { get;  set; }
        public Guid ShipmentId { get;  set; }
        public decimal ShippingCost { get; set; }
        public decimal Commission { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;  
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; init; }
        public string? Notes { get; init; }  
    }

}
