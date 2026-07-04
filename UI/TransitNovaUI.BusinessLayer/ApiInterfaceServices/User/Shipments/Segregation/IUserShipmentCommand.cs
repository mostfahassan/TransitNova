using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation
{
    public interface IUserShipmentsCommand : ICancelShipmentCommandService, ICreateShipmentCommandService, IDeleteShipmentCommandService, IIssueShipmentCommandService, IUpdateShipmentCommandService
    {
    }

    public interface IUserShipmentCommand : IUserShipmentsCommand
    {
    }
}
