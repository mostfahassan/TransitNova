using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation
{
    internal interface ICarrierShipmentCommand: ICompleteDeliveryCommandService, ICompletePickupCommandService, IUpdateCarrierStatusCommandService
    {
    }
}
