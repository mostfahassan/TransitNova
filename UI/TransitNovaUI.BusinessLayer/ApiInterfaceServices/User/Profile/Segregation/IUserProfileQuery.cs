using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation
{
    internal interface IUserProfileQuery : IGetUserDashboardQueryService, IGetUserProfileQueryService
    {
    }
}
