using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
 
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands
{
    public sealed class CreateVehicleHandler(
        IVehicleQueryRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateVehicleHandler> logger)
        : ICommandHandler<CreateVehicleCommand, Result<VehicleDto>>
    {
        public async Task<Result<VehicleDto>> Handle(CreateVehicleCommand request, CancellationToken ct)
        {
            var plateNumber = request.Dto.PlateNumber.Trim();
            var vehicle = Vehicle.Create(request.Dto.VehicleType, plateNumber, request.Dto.CapacityWeight, request.Dto.CapacityVolume, request.Dto.IsRefrigerated, request.Dto.CarrierId);

            await vehicleRepository.AddAsync(vehicle, ct);
            
            await unitOfWork.SaveChangesAsync(ct);
           

            var createdVehicle = await vehicleRepository.GetByIdAsync<VehicleDto>(vehicle.Id, ct);
            if (createdVehicle is null)
            {
                logger.LogWarning("Vehicle created but could not be retrieved. VehicleId: {VehicleId}", vehicle.Id);
                return Result<VehicleDto>.Failure(Errors.NotFound("Vehicle creation failed."));
            }

            logger.LogInformation("Vehicle created successfully. VehicleId: {VehicleId}", vehicle.Id);
            return Result<VehicleDto>.Created(createdVehicle);
        }
    }
}
