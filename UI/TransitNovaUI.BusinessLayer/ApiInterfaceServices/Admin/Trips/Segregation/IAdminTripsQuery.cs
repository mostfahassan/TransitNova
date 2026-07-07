using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Segregation
{
    public interface IAdminTripsQuery : IGetAdminTripsQueryService, IGetAdminTripByIdQueryService
    {
    }
}