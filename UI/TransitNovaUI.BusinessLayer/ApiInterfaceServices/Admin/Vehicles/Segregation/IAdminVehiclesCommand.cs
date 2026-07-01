using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation
{
    internal interface IAdminVehiclesCommand : ICreateVehicleCommandService, IDeleteVehicleCommandService
    {
    }
}
