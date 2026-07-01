using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation
{
    internal interface IUserShipmentQuery : IGetUserShipmentByIdQueryService, ITrackShipmentQueryService
    {
    }
}
