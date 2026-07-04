using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation
{
    public interface IAdminWarehousesCommand : ICreateWarehouseCommandService, IDeleteWarehouseCommandService, IUpdateWarehouseCommandService
    {
    }
}

