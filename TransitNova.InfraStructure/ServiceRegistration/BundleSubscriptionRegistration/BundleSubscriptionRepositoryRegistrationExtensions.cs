using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscription;
using TransitNova.InfraStructure.Repository.BundleSubscriptions;

namespace TransitNova.InfraStructure.ServiceRegistration.BundleSubscriptionRegistration
{
    public static class BundleSubscriptionRepositoryRegistrationExtensions
    {

        public static IServiceCollection AddBundleSubscriptionRepositories(
            this IServiceCollection services)
        {

            services.AddScoped<IBundleSubscriptionQueryRepository, BundleSubscriptionQueryRepository>();
            return services;
        }
    }
}
