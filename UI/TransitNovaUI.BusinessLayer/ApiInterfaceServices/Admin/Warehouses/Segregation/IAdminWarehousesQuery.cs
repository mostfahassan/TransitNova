using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation
{
    public interface IAdminWarehousesQuery : IGetWarehouseByIdQueryService, IGetWarehousesQueryService
    {
    }
}

