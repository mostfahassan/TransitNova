using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Query
{
    public interface ICarrierTripsQuery : IGetCarrierTripByIdQueryService, IGetCarrierTripsQueryService
    {
    }

    public interface ITripsCarrierTripsQuery : ICarrierTripsQuery
    {
    }
}
