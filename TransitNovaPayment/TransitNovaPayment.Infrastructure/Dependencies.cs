using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace TransitNovaPayment.Busieness
{
    public static class Dependencies
    {
        public static IServiceCollection AddBuisnessDependencies(this IServiceCollection services,IConfiguration configuration)
        {

            return services;
        }
    }
}
