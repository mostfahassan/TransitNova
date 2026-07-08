using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Segregation;

public interface ISharedNotificationsQuery : IGetNotificationsQueryService, IGetUnreadCountQueryService
{
}