using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation
{
    public interface ICarrierShipmentsCommand : ICompleteDeliveryCommandService, ICompletePickupCommandService, IMarkShipmentPickedUpCommandService, IUpdateCarrierStatusCommandService
    {
    }

    public interface ICarrierShipmentCommand : ICarrierShipmentsCommand
    {
    }
}
