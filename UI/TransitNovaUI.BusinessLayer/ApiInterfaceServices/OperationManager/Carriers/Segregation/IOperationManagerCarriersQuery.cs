using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation
{
    public interface IOperationManagerCarriersQuery : IFilterCarriersQueryService, IGetCarrierByIdQueryService, IGetCarrierShipmentByIdQueryService, IGetCarrierShipmentsQueryService
    {
    }
}
