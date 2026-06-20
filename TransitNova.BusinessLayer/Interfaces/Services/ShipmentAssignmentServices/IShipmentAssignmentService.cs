namespace TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices
{
    public interface IShipmentAssignmentService
    {
        Task<string> AssignDelivery(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId , CancellationToken cancellationToken);
        Task<string> AssignPickup(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId ,CancellationToken cancellationToken);

    }
}
