using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Dashboard.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Profile.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Shipments.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Shipments.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.OperationManagerRegistration;

public static class OperationManagerApiClientRegistrationExtensions
{
    public static IServiceCollection AddOperationManagerApiClients(this IServiceCollection services)
    {
        services.AddScoped<OperationManagerCarriersCommand>();
        services.AddScoped<IOperationManagerCarriersCommand, OperationManagerCarriersCommand>();
        services.AddScoped<IAssignDeliveryCarrierCommandService, OperationManagerCarriersCommand>();
        services.AddScoped<IAssignPickupCarrierCommandService, OperationManagerCarriersCommand>();

        services.AddScoped<OperationManagerCarriersQuery>();
        services.AddScoped<IOperationManagerCarriersQuery, OperationManagerCarriersQuery>();
        services.AddScoped<IFilterCarriersQueryService, OperationManagerCarriersQuery>();
        services.AddScoped<IGetCarrierByIdQueryService, OperationManagerCarriersQuery>();
        services.AddScoped<IGetCarrierShipmentByIdQueryService, OperationManagerCarriersQuery>();
        services.AddScoped<IGetCarrierShipmentsQueryService, OperationManagerCarriersQuery>();

        services.AddScoped<OperationManagerDashboardQuery>();
        services.AddScoped<IOperationManagerDashboardQuery, OperationManagerDashboardQuery>();
        services.AddScoped<IGetOperationManagerDashboardQueryService, OperationManagerDashboardQuery>();

        services.AddScoped<OperationManagerProfileQuery>();
        services.AddScoped<IGetHandledCarriersQueryService, OperationManagerProfileQuery>();
        services.AddScoped<IGetHandledShipmentsQueryService, OperationManagerProfileQuery>();
        services.AddScoped<IGetOperationManagerByIdQueryService, OperationManagerProfileQuery>();

        services.AddScoped<OperationManagerShipmentsCommand>();
        services.AddScoped<IOperationManagerShipmentsCommand, OperationManagerShipmentsCommand>();
        services.AddScoped<IApproveShipmentCommandService, OperationManagerShipmentsCommand>();
        services.AddScoped<IRejectShipmentCommandService, OperationManagerShipmentsCommand>();

        services.AddScoped<OperationManagerShipmentsQuery>();
        services.AddScoped<IOperationManagerShipmentsQuery, OperationManagerShipmentsQuery>();
        services.AddScoped<IFilterShipmentsQueryService, OperationManagerShipmentsQuery>();
        services.AddScoped<IGetAssignedShipmentsQueryService, OperationManagerShipmentsQuery>();
        services.AddScoped<IGetShipmentByIdQueryService, OperationManagerShipmentsQuery>();
        services.AddScoped<IGetShipmentHistoriesQueryService, OperationManagerShipmentsQuery>();
        services.AddScoped<IGetShipmentReviewQueueQueryService, OperationManagerShipmentsQuery>();
        services.AddScoped<IReviewShipmentQueryService, OperationManagerShipmentsQuery>();

        return services;
    }
}
