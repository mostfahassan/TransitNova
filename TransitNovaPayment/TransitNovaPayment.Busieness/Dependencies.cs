using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction;
using TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods;
using TransitNovaPayment.Busieness.Common.Behaviour;
using TransitNovaPayment.Busieness.Implementation;
using TransitNovaPayment.Busieness.Implementation.PaymentService;
using TransitNovaPayment.Busieness.Interfaces.PaymentExecution;
using TransitNovaPayment.Busieness.Interfaces.PaymentService;

namespace TransitNovaPayment.Busieness
{
    public static class Dependencies
    {
        public static IServiceCollection AddBuisnessDependencies(this IServiceCollection services)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(Dependencies).Assembly));

            services.AddValidatorsFromAssembly(typeof(Dependencies).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            services.AddScoped<IShipmentPayment, ShipmentPaymentProcess>();
            services.AddScoped<IBundlePayment, BundlePaymentProcess>();
            services.AddScoped<IPaymentExecutionStrategy, RandomizedPaymentExecutionStrategy>();
            services.AddScoped<PaymentMethodService, CreditCard>();
            services.AddScoped<PaymentMethodService, PaypalPayment>();
            services.AddScoped<PaymentMethodService, WalletsPayment>();

            return services;
        }
    }
}

