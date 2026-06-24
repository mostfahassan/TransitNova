using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.VehicleRepo.VehicleRules
{
    internal class VehicleRulesRepository(AppDbContext context) : IVehicleRulesRepository
    {
        private readonly DbSet<Vehicle> _vehicles = context.Set<Vehicle>();
        public async Task<bool> ExistsByIdAsync(Guid vehicleId, CancellationToken ct)
            => await _vehicles.AsNoTracking().AnyAsync(v => v.Id == vehicleId, ct);
        public async Task<bool> ExistsByPlateNumberAsync(string plateNumber, CancellationToken ct)
            => await _vehicles.AsNoTracking().AnyAsync(v => v.PlateNumber == plateNumber, ct);
        public async Task<bool> PlateNumberExistsForAnotherVehicleAsync(Guid vehicleId, string plateNumber, CancellationToken ct)
            => await _vehicles
                .AsNoTracking()
                .AnyAsync(v => v.Id != vehicleId && v.PlateNumber == plateNumber, ct);
        public async Task<bool> CarrierHasVehicleAsync(Guid carrierId, CancellationToken ct)
            => await _vehicles
                .AsNoTracking()
                .AnyAsync(v => v.CarrierId == carrierId, ct);
        public async Task<bool> CarrierHasAnotherVehicleAsync(Guid carrierId, Guid vehicleId, CancellationToken ct)
            => await _vehicles
                .AsNoTracking()
                .AnyAsync(v => v.CarrierId == carrierId && v.Id != vehicleId, ct);

      
    }
}
