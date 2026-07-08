using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Abstract;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using PaymentEntity = TransitNovaPayment.Busieness.Models.PaymentEntity.Payment;
using PaymentHistoryEntity = TransitNovaPayment.Busieness.Models.PaymentHistoryEntity.PaymentHistory;

namespace TransitNova.Payment.Tests.TestInfrastructure;

internal static class PaymentTestData
{
    internal static CreatePaymentDto CreatePaymentDto(
        PaymentMethod paymentMethod = PaymentMethod.CreditCard,
        decimal shippingCost = 100m,
        Guid? shipmentId = null)
    {
        return new CreatePaymentDto
        {
            ShipmentId = shipmentId ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
            PaymentMethod = paymentMethod,
            ShippingCost = shippingCost
        };
    }

    internal static IOptions<PaymentGatewaySettings> CreatePaymentGatewayOptions(string? privateKey = "payment-private-key") =>
        Options.Create(new PaymentGatewaySettings { PrivateKey = privateKey });

    internal static PaymentEntity CreatePayment(
        PaymentMethod paymentMethod = PaymentMethod.CreditCard,
        decimal totalAmount = 100m,
        Guid? shipmentId = null)
    {
        var payment = PaymentEntity.Create(
            totalAmount,
            shipmentId ?? Guid.Parse("22222222-2222-2222-2222-222222222222"),
            paymentMethod);

        typeof(PaymentEntity)
            .GetProperty("RowVersion")!
            .SetValue(payment, new byte[] { 1 });

        return payment;
    }

    internal static PaymentHistoryEntity CreateHistory(
        PaymentEntity payment,
        PaymentStatus oldStatus,
        PaymentStatus newStatus,
        DateTime createdAt,
        string createdBy)
    {
        var history = new PaymentHistoryEntity
        {
            Payment = payment,
            PaymentId = payment.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = createdAt.AddMinutes(5)
        };

        typeof(PaymentHistoryEntity).BaseType!
            .GetProperty("CreatedAt")!
            .SetValue(history, createdAt);
        typeof(PaymentHistoryEntity).BaseType!
            .GetProperty("CreatedBy")!
            .SetValue(history, createdBy);

        return history;
    }

    internal static FixedPaymentMethodService FixedPaymentMethod(
        PaymentMethod paymentMethod,
        decimal commission)
    {
        return new FixedPaymentMethodService(paymentMethod, commission);
    }

    internal sealed class FixedPaymentMethodService(PaymentMethod paymentMethod, decimal commission) : PaymentMethodService
    {
        public override decimal Commision => commission;

        public override PaymentMethod PaymentMethod => paymentMethod;

        public override decimal Pay(decimal shippingCost,Currency currency)
        {
            var rate = currency switch
            {
                Currency.USD => 1m,
                Currency.EUR => 0.9m,
                Currency.EGB => 30m,
                _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
            };
            return shippingCost + (shippingCost * commission / rate);
        }
    }
}
