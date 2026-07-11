using TransitNova.Domain.Enums.Payment;

namespace TransitNova.BusinessLayer.DTOs.BundleSubscription;

public sealed class SubscribeToBundleDto
{
    public PaymentMethod PaymentMethod { get; set; }
}
