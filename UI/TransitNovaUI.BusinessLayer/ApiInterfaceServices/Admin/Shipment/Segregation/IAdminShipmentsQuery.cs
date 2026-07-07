using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Query;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Segregation
{
    public interface IAdminShipmentsQuery: IGetAdminShipmentsQueryService , IGetAdminShipmentsByIdQueryService
    {
    }
}
