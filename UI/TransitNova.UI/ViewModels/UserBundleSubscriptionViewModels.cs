using System.ComponentModel.DataAnnotations;
using TransitNova.Domain.Enums.Payment;
using TransitNovaUI.BusinessLayer.DTOs.Bundle;
using TransitNovaUI.BusinessLayer.DTOs.BundleSubscription;

namespace TransitNova.UI.ViewModels;

public sealed class BundleSubscriptionPaymentViewModel
{
    [Required]
    public Guid BundleId { get; set; }

    public UiRetrieveBundleDto? Bundle { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    public UiSubscribeToBundleDto ToDto() => new()
    {
        PaymentMethod = PaymentMethod
    };
}
