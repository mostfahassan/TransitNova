using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation
{
    internal interface IOperationManagerShipmentsCommand : IApproveShipmentCommandService, IRejectShipmentCommandService
    {
    }
    internal interface IOperationManagerShipmentsQuery
        : IFilterShipmentsQueryService, IGetAssignedShipmentsQueryService,
          IGetShipmentByIdQueryService, IGetShipmentHistoriesQueryService, IGetShipmentReviewQueueQueryService, IReviewShipmentQueryService
    {

    }
}
