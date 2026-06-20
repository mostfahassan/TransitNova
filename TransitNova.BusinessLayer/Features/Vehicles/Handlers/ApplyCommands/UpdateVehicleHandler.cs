using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands
{
    public sealed class UpdateVehicleHandler(
        IVehicleQueryRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateVehicleHandler> logger)
        : ICommandHandler<UpdateVehicleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateVehicleCommand request, CancellationToken ct)
        {
            var vehicle = await vehicleRepository.GetByIdAsync<Vehicle?>(request.Id, ct);
            var plateNumber = request.Dto.PlateNumber.Trim();

            vehicle!.UpdateVehicle(
                request.Dto.VehicleType,
                plateNumber,
                request.Dto.CapacityWeight,
                request.Dto.CapacityVolume,
                request.Dto.IsRefrigerated);

            if (vehicle.CarrierId != request.Dto.CarrierId)
            {
                vehicle.ChangeCarrier(request.Dto.CarrierId);
            }

            vehicleRepository.Update(vehicle);
            
            await unitOfWork.SaveChangesAsync(ct);
           

            logger.LogInformation("Vehicle updated successfully. VehicleId: {VehicleId}", request.Id);
            return BaseResult.Success();
        }
    }
}
