using FluentAssertions;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Busieness.Validation.PaymentValidators;

namespace TransitNova.Payment.Tests.Validators;

public sealed class CreatePaymenetDtoValidatorTests
{
    private readonly CreatePaymenetDtoValidator _validator = new();

    [Fact]
    public void Validate_WhenShipmentIdIsEmpty_ShouldReturnValidationError()
    {
        var result = _validator.Validate(PaymentTestData.CreatePaymentDto(shipmentId: Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == "ShipmentId");
    }

    [Fact]
    public void Validate_WhenShippingCostHasTooManyDecimalPlaces_ShouldReturnValidationError()
    {
        var result = _validator.Validate(PaymentTestData.CreatePaymentDto(shippingCost: 12.345m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "ShippingCost");
    }

    [Fact]
    public void Validate_WhenDtoIsValid_ShouldPass()
    {
        var result = _validator.Validate(PaymentTestData.CreatePaymentDto(paymentMethod: PaymentMethod.PayPal));

        result.IsValid.Should().BeTrue();
    }
}
