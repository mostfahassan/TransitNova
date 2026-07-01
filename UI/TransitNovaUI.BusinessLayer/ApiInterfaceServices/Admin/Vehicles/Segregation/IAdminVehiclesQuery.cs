using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation
{
    internal interface IAdminVehiclesQuery: IGetActiveVehiclesQueryService, IGetVehicleByIdQueryService, IGetVehicleByPlateNumberQueryService, IGetVehiclesQueryService
    {
    }
}
