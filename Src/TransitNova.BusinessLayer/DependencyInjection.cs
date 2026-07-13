using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Common.Behaviors;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.BusinessLayer.Interfaces.Services.CarrierDashboard;
using TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Interfaces.Services.OperationManagerDashboard;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.TokenServices;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.BusinessLayer.Interfaces.Services.WarehouseManagerDashboardService;
using TransitNova.BusinessLayer.Services.AdminDashboardService;
using TransitNova.BusinessLayer.Services.BundleService;
using TransitNova.BusinessLayer.Services.CarrierDashboardService;
using TransitNova.BusinessLayer.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Services.OperationManagerDashboardService;
using TransitNova.BusinessLayer.Services.PaymentServices;
using TransitNova.BusinessLayer.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Services.ShipmentServices;
using TransitNova.BusinessLayer.Services.TokenServices;
using TransitNova.BusinessLayer.Services.TripServices;
using TransitNova.BusinessLayer.Services.WarehouseManagerDashboardService;
namespace TransitNova.BusinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInBusinessService(
          this IServiceCollection services)

        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.
                AddMediatR(options => options.RegisterServicesFromAssembly(assembly))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotentCommandPipelineBehavior<,>))
                .AddAutoMapper(config =>
                  {
                      config.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
                  })
                .AddValidatorsFromAssembly(assembly)
                .AddScoped<IShipmentPricingServices, ShipmentPricingServices>()
                .AddScoped<ICompleteShipmentService, CompleteShipmentService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IShipmentAssignmentService, ShipmentAssignmentService>()
                .AddScoped<IShipmentService, ShipmentService>()
                .AddScoped<IWarehouseManagerDashboard, WarehouseManagerDashboardBuilder>()
                .AddScoped<ITripServices, TripManagementService>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IPaymentHistoryService, PaymentHistoryService>()
                .AddScoped<IAdminDashboard, AdminDashboard>()
                .AddScoped<IOperationManagerDashboard, OperationManagerDashboard>()
                .AddScoped<ICarrierDashboard, CarrierDashboard>()
                .AddScoped<IBundleBenefitService, BundleBenefitService>()
                .AddScoped<IBundleSubscription, BundleSubscriptionPayment>();


            return services;
        }
    }
}

