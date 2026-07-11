using TransitNova.Domain.Enums.Payment;

namespace TransitNovaUI.BusinessLayer.DTOs.BundleSubscription;

public sealed class UiSubscribeToBundleDto
{
    public PaymentMethod PaymentMethod { get; set; }
}
