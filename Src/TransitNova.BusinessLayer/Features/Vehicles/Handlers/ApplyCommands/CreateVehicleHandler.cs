using AutoMapper;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands
{
    public sealed class CreateVehicleHandler(
        IVehicleQueryRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<CreateVehicleHandler> logger)
        : ICommandHandler<CreateVehicleCommand, Result<VehicleDto>>
    {
        public async Task<Result<VehicleDto>> Handle(CreateVehicleCommand request, CancellationToken ct)
        {
            var vehicle = mapper.Map<Vehicle>(request.Dto);

            await vehicleRepository.AddAsync(vehicle, ct);
            
            await unitOfWork.SaveChangesAsync(ct);
           

            var createdVehicle = await vehicleRepository.GetByIdAsync<VehicleDto>(vehicle.Id, ct);
            if (createdVehicle is null)
            {
                logger.LogWarning("Vehicle created but could not be retrieved. VehicleId: {VehicleId}", vehicle.Id);
                return Result<VehicleDto>.Failure(Errors.NotFound("Vehicle creation failed."));
            }

            logger.LogInformation("Vehicle created successfully. VehicleId: {VehicleId}", vehicle.Id);

            await cacheService.RemoveAsync(CacheKeys.VehicleList());
            return Result<VehicleDto>.Created(createdVehicle);
        }
    }
}
