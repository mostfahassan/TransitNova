using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Segregation
{
    internal interface IOperationManagerProfileQuery : IGetHandledCarriersQueryService, IGetHandledShipmentsQueryService, IGetOperationManagerByIdQueryService
    { 
    }
}
