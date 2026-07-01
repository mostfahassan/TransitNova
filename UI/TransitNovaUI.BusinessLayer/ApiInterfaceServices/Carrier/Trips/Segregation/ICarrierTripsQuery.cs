using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Segregation
{
    internal interface ICarrierTripsQuery : IGetCarrierTripByIdQueryService, IGetCarrierTripsQueryService
    {
    }
}
