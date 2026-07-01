using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation
{
    internal interface IOperationManagerCarriersQueries : IFilterCarriersQueryService, IGetCarrierByIdQueryService, IGetCarrierShipmentByIdQueryService, IGetCarrierShipmentsQueryService
    {
    }
}
