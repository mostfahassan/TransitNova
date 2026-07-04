using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation
{
    public interface IOperationManagerShipmentsCommand : IApproveShipmentCommandService, IRejectShipmentCommandService
    {
    }
    public interface IOperationManagerShipmentsQuery
        : IFilterShipmentsQueryService, IGetAssignedShipmentsQueryService,
          IGetShipmentByIdQueryService, IGetShipmentHistoriesQueryService, IGetShipmentReviewQueueQueryService, IReviewShipmentQueryService
    {

    }
}

