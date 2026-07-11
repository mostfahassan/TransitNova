namespace TransitNova.BusinessLayer.DTOs.BundleSubscription
{
    public class BundleSubscriptionDetailsDto
    {
        public Guid Id { get; set; }
        public Guid BundleId { get; set; }
        public string BundleName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancelledAt { get; set; }
    }

    public class BundleSubscriptionInvoice
    {
        public Guid Id { get; set; }
        public string BundleName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal BundlePrice { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SubscribedAt { get; set; }
    }
}
