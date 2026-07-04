using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Segregation
{
    public interface IAdminOperationManagerQuery : IGetActiveOperationManagersQueryService,
        IGetOperationManagerByIdQueryService,
        IGetOperationManagerHandledCarriersQueryService, 
        IGetOperationManagerHandledShipmentsQueryService, 
        IGetOperationManagersQueryService
    {
    }
}

