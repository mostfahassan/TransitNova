namespace TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository
{
    public interface IVehicleRulesRepository
    {
        Task<bool> ExistsByIdAsync(Guid vehicleId, CancellationToken ct);
        Task<bool> ExistsByPlateNumberAsync(string plateNumber, CancellationToken ct);
        Task<bool> PlateNumberExistsForAnotherVehicleAsync(Guid vehicleId, string plateNumber, CancellationToken ct);
        Task<bool> CarrierHasVehicleAsync(Guid carrierId, CancellationToken ct);
        Task<bool> CarrierHasAnotherVehicleAsync(Guid carrierId, Guid vehicleId, CancellationToken ct);
    }
}
