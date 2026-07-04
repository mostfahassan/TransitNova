using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation
{
    public interface IAdminSubscriptionQuery : IGetBundleSubscribersQueryService, IGetSubscriptionByIdQueryService
    {
    }
}

