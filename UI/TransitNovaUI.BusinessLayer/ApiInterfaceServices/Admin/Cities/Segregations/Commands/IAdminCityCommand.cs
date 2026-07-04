using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands
{
    public interface IAdminCityCommand : ICreateCityCommandService, IDeleteCityCommandService, IUpdateCityCommandService
    {
    }
}
