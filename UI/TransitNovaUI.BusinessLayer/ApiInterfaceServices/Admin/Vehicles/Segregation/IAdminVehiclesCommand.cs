using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation
{
    public interface IAdminVehiclesCommand : ICreateVehicleCommandService, IDeleteVehicleCommandService
    {
    }
}

