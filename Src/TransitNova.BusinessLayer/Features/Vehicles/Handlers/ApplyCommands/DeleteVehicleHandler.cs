using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands
{
    public sealed class DeleteVehicleHandler(
        IVehicleQueryRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<DeleteVehicleHandler> logger)
        : ICommandHandler<DeleteVehicleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteVehicleCommand request, CancellationToken ct)
        {
            var vehicle = await vehicleRepository.GetByIdAsync<Vehicle>(request.Id, ct);
            if (vehicle == null)
                return BaseResult.NotFound(Errors.NotFound("City Not Found"));


            var deleted = await vehicleRepository.DeleteAsync(request.Id, ct);
            if (!deleted)
            {
                logger.LogWarning("Vehicle delete failed because Vehicle was not found.VehicleId: {VehicleId}", request.Id);
                return BaseResult.UnExpected(Errors.FailedOperation("An error occurred while removing the vehicle."));
            }

            await unitOfWork.SaveChangesAsync(ct);

            await cacheService.RemoveAsync(CacheKeys.VehicleList());

            logger.LogInformation("Vehicle deleted successfully. VehicleId: {VehicleId}", request.Id);
            return BaseResult.Success();
        }
    }
}
