using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.Domain.Enums.Payment;

namespace TransitNovaUI.BusinessLayer.DTOs.Payment;

public sealed class UiPaymentSummaryDto
{
    public PaymentMethod PaymentMethod { get; set; }

    public static UiPaymentSummaryDto ToUiDto(PaymentSummaryDto source) =>
        new() { PaymentMethod = source.PaymentMethod };
}
