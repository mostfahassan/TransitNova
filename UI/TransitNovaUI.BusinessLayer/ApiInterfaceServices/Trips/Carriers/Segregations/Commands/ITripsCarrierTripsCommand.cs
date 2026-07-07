using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Commands
{
    public interface ICarrierTripsCommand : ICompleteCarrierTripCommandService
    {
    }

    public interface ITripsCarrierTripsCommand : ICarrierTripsCommand
    {
    }
}