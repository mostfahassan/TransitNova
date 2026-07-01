using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation
{
    internal interface IAdminSubscriptionQuery : IGetBundleSubscribersQueryService, IGetSubscriptionByIdQueryService
    {
    }
}
