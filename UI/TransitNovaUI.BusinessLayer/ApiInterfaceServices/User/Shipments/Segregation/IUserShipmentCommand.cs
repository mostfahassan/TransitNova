using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation
{
    internal interface IUserShipmentCommand : ICancelShipmentCommandService, ICreateShipmentCommandService, IDeleteShipmentCommandService, IIssueShipmentCommandService, IUpdateShipmentCommandService
    {
    }
}
