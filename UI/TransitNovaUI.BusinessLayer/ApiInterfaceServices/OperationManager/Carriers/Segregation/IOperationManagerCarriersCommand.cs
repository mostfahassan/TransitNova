using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation
{
    internal interface IOperationManagerCarriersQuery : IAssignDeliveryCarrierCommandService, IAssignPickupCarrierCommandService
    {
    }
}
