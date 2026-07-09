using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.CarrierRatings.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.Profile.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.PaymentInvoices.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.User.Subscriptions.Command;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.UserRegistration;

public static class UserApiClientRegistrationExtensions
{
    public static IServiceCollection AddUserApiClients(this IServiceCollection services)
    {
        services.AddScoped<UserCarrierRatingsCommand>();
        services.AddScoped<IRateDeliveryCarrierCommandService, UserCarrierRatingsCommand>();
        services.AddScoped<IRatePickupCarrierCommandService, UserCarrierRatingsCommand>();

        services.AddScoped<UserProfileQuery>();
        services.AddScoped<IUserProfileQuery, UserProfileQuery>();
        services.AddScoped<IGetUserDashboardQueryService, UserProfileQuery>();
        services.AddScoped<IGetUserProfileQueryService, UserProfileQuery>();

        services.AddScoped<UserShipmentsCommand>();
        services.AddScoped<IUserShipmentsCommand, UserShipmentsCommand>();
        services.AddScoped<IUserShipmentCommand, UserShipmentsCommand>();
        services.AddScoped<ICancelShipmentCommandService, UserShipmentsCommand>();
        services.AddScoped<ICreateShipmentCommandService, UserShipmentsCommand>();
        services.AddScoped<IDeleteShipmentCommandService, UserShipmentsCommand>();
        services.AddScoped<IIssueShipmentCommandService, UserShipmentsCommand>();
        services.AddScoped<IUpdateShipmentCommandService, UserShipmentsCommand>();

        services.AddScoped<UserPaymentInvoicesQuery>();
        services.AddScoped<IUserPaymentInvoicesQuery, UserPaymentInvoicesQuery>();
        services.AddScoped<IGetUserPaymentInvoiceQueryService, UserPaymentInvoicesQuery>();
        services.AddScoped<IGetUserPaymentInvoicesQueryService, UserPaymentInvoicesQuery>();

        services.AddScoped<UserShipmentsQuery>();
        services.AddScoped<IUserShipmentsQuery, UserShipmentsQuery>();
        services.AddScoped<IUserShipmentQuery, UserShipmentsQuery>();
        services.AddScoped<IGetUserShipmentByIdQueryService, UserShipmentsQuery>();
        services.AddScoped<ITrackShipmentQueryService, UserShipmentsQuery>();

        services.AddScoped<UserSubscriptionsCommand>();
        services.AddScoped<IUserSubscriptionCommand, UserSubscriptionsCommand>();
        services.AddScoped<ISubscribeToBundleCommandService, UserSubscriptionsCommand>();
        services.AddScoped<IUnsubscribeFromBundleCommandService, UserSubscriptionsCommand>();

        return services;
    }
}


