using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation
{
    internal interface ICarrierShipmentQuery : IGetCarrierShipmentByIdQueryService, IGetCarrierShipmentsQueryService
    {
    }
}
