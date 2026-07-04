using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation
{
    public interface ICarrierShipmentsQuery : IGetCarrierShipmentByIdQueryService, IGetCarrierShipmentsQueryService
    {
    }

    public interface ICarrierShipmentQuery : ICarrierShipmentsQuery
    {
    }
}
