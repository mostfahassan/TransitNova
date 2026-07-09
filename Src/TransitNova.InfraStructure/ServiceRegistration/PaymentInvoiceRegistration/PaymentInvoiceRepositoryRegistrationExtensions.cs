using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.InfraStructure.Repository.PaymmentInvoice;

namespace TransitNova.InfraStructure.ServiceRegistration.PaymentInvoiceRegistration
{
    public static class PaymentInvoiceRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddPaymentInvoiceRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRepositoryCommand, PaymentRepositoryCommand>();
            services.AddScoped<IPaymentRepositoryQuery, PaymentRepositoryQuery>();

            return services;
        }
    }
}
