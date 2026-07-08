using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.Notification;
using TransitNova.InfraStructure.Repository.Notifications;
using TransitNova.InfraStructure.SignalR;

namespace TransitNova.InfraStructure.ServiceRegistration.NotificationRegistration
{
    public static class NotificationRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddNotificationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationCommand, NotificationCommand>()
                .AddScoped<INotificationQueryRepository, NotificationQueryRepository>()
                .AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            services.AddSignalR();
            return services;
        }
    }
}