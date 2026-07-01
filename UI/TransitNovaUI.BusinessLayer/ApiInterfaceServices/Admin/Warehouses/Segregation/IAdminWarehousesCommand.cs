using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation
{
    internal interface IAdminWarehousesCommand : ICreateWarehouseCommandService, IDeleteWarehouseCommandService, IUpdateWarehouseCommandService
    {
    }
}
