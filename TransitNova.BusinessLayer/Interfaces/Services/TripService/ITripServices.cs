
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Services.TripService
{
    public interface ITripServices
    {
        Task<Trip> StartDeliveryTripAsync(Guid OperationManagerId, Guid CarrierId, CancellationToken cancellationToken);
        Task<Trip> StartPickUpTripAsync(Guid OperationManagerId, Guid CarrierId, CancellationToken cancellationToken);
    }
}
