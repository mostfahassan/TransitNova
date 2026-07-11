using FluentAssertions;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Services.Payment.Command;
using TransitNovaPayment.Busieness.Services.Payment.Command.CommandValidator;
using TransitNovaPayment.Busieness.Validation.PaymentValidators;
namespace TransitNova.Payment.Tests.Validators;

public sealed class CreatePaymentCommandValidatorTests
{
    private readonly CreatePaymentCommandValidator _validator = new(new CreatePaymenetDtoValidator());
    private readonly CreateBundlePaymentCommandValidator _bundleValidator = new(new CreatePaymenetDtoValidator());

    [Fact]
    public void Validate_WhenPublicKeyIsMissing_ShouldReturnUnauthorizedErrorCode()
    {
        var command = new CreateShipmentPaymentCommand(PaymentTestData.CreatePaymentDto(), string.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == "Key" && error.ErrorCode == "UNAUTHORIZED");
    }

    [Fact]
    public void Validate_WhenDtoIsInvalid_ShouldIncludeNestedValidationErrors()
    {
        var command = new CreateShipmentPaymentCommand(PaymentTestData.CreatePaymentDto(shippingCost: 0m), "key");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Dto.Cost");
    }
}

