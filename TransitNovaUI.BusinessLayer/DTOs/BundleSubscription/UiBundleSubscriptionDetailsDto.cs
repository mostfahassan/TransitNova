using TransitNova.BusinessLayer.DTOs.BundleSubscription;

namespace TransitNovaUI.BusinessLayer.DTOs.BundleSubscription;

public sealed class UiBundleSubscriptionDetailsDto
{
    public Guid Id { get; set; }
    public int BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CancelledAt { get; set; }

    public static UiBundleSubscriptionDetailsDto ToUiDto(BundleSubscriptionDetailsDto source) =>
        new()
        {
            Id = source.Id,
            BundleId = source.BundleId,
            BundleName = source.BundleName,
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            IsActive = source.IsActive,
            SubscriptionDate = source.SubscriptionDate,
            EndDate = source.EndDate,
            CancelledAt = source.CancelledAt
        };
}
