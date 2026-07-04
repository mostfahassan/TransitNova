using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Segregations.Commands
{
    public interface IOperationManagerTripsCommand : IStartDeliveryTripCommandService, IStartPickupTripCommandService
    {
    }

    public interface ITripsOperationManagerTripsCommand : IOperationManagerTripsCommand
    {
    }
}
