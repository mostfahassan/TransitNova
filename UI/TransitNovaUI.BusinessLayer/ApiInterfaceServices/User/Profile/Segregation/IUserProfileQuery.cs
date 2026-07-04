using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation
{
    public interface IUserProfileQuery : IGetUserDashboardQueryService, IGetUserProfileQueryService
    {
    }
}

