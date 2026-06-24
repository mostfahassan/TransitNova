namespace TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices
{
    public interface IShipmentAssignmentService
    {
        Task<string> AssignDeliveryAsync(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId , CancellationToken cancellationToken);
        Task<string> AssignPickupAsync(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId ,CancellationToken cancellationToken);

    }
}
