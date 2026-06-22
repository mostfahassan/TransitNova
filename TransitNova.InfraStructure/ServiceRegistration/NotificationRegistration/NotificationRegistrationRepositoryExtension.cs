using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.Notification;
using TransitNova.InfraStructure.Common.NotificationService;
using TransitNova.InfraStructure.Repository.Notifications;
namespace TransitNova.InfraStructure.ServiceRegistration.NotificationRegistration
{
    public static class NotificationRegistrationRepositoryExtension
    {
        public static IServiceCollection AddNotificationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationCommand, NotificationCommand>()
                .AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            services.AddSignalR();
            return services;
        }
    }
}
