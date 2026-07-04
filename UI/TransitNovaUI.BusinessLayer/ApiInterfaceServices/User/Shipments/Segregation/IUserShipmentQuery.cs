using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation
{
    public interface IUserShipmentsQuery : IGetUserShipmentByIdQueryService, ITrackShipmentQueryService
    {
    }

    public interface IUserShipmentQuery : IUserShipmentsQuery
    {
    }
}
