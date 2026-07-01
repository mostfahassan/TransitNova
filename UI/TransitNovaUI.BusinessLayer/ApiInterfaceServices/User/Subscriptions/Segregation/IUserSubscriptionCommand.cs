using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation
{
    internal interface IUserSubscriptionCommand : ISubscribeToBundleCommandService, IUnsubscribeFromBundleCommandService
    {
    }
}
