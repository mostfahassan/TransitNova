using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation
{
    public interface IUserSubscriptionCommand : ISubscribeToBundleCommandService, IUnsubscribeFromBundleCommandService
    {
    }
}

