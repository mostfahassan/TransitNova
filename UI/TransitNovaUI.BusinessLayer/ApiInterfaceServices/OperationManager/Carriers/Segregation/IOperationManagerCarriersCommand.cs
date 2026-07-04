using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation
{
    public interface IOperationManagerCarriersCommand : IAssignDeliveryCarrierCommandService, IAssignPickupCarrierCommandService
    {
    }
}
