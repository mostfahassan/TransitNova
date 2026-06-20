using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;

namespace TransitNova.InfraStructure.Repository.VehicleRepo
{
    public class VehicleQueryRepository(AppDbContext context, IConfigurationProvider mapperConfig)
        : GenericRepository<Vehicle, Guid>(context, mapperConfig), IVehicleQueryRepository
    {
        private readonly DbSet<Vehicle> vehicle = context.Set<Vehicle>();
        public async Task<List<VehicleDto>> GetActiveAsync(CancellationToken ct)
            => await vehicle
                .AsNoTracking()
                .Where(v => v.IsActive)
                .ProjectTo<VehicleDto>(mapperConfig)
                .ToListAsync(ct);
        public async Task<List<VehicleDto>> GetByCarrierIdAsync(Guid carrierId, CancellationToken ct)
            => await vehicle
                .AsNoTracking()
                .Where(v => v.CarrierId == carrierId)
                .ProjectTo<VehicleDto>(mapperConfig)
                .ToListAsync(ct);
        public async Task<VehicleDto?> GetVehicleDetailsAsync(Guid id, CancellationToken ct)
            => await context.Vehicles.AsQueryable()
                .AsNoTracking()
                .Where(v => v.Id == id)
                .ProjectTo<VehicleDto>(mapperConfig)
                .FirstOrDefaultAsync(ct);
             
       
        public async Task<VehicleDto?> GetByPlateNumberAsync(string plateNumber, CancellationToken ct)
            => await vehicle
                .AsNoTracking()
                .Where(v => v.PlateNumber == plateNumber)
                .ProjectTo<VehicleDto>(mapperConfig)
                .FirstOrDefaultAsync(ct);
    }
}
