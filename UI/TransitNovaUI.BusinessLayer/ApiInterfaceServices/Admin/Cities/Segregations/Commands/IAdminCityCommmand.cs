using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands
{
    internal interface IAdminCityCommmand : ICreateCityCommandService, IDeleteCityCommandService, IUpdateCityCommandService
    {
    }
}
