using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation
{
    public interface IOperationManagerTripsQuery : IGetOperationManagerTripsQueryService, IGetOperationManagerTripByIdQueryService
    {
    }
}