using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TransitNovaPayment.Busieness;
using TransitNovaPayment.Busieness.Implementation.PaymentService;
using TransitNovaPayment.Busieness.Interfaces.PaymentService;
using TransitNovaPayment.Busieness.Services.Payment.Command;
using TransitNovaPayment.Busieness.Services.Payment.Command.CommandValidator;

namespace TransitNova.Payment.Tests.DependencyInjection;

public sealed class PaymentDependencyRegistrationTests
{
    [Fact]
    public void AddBuisnessDependencies_ShouldRegisterBundlePaymentServicesAsync()
    {
        var services = new ServiceCollection();

        services.AddBuisnessDependencies();

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IBundlePayment)
            && descriptor.ImplementationType == typeof(BundlePaymentProcess)
            && descriptor.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<CreateBundlePaymentCommand>)
            && descriptor.ImplementationType == typeof(CreateBundlePaymentCommandValidator));
    }
}
