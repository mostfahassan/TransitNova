using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation
{
    public interface ICarrierShipmentsCommand : ICompleteDeliveryCommandService, ICompletePickupCommandService, IUpdateCarrierStatusCommandService
    {
    }

    public interface ICarrierShipmentCommand : ICarrierShipmentsCommand
    {
    }
}
