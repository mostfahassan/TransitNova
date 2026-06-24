
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Crud
{
    public sealed class AddCarrierAdditionalInfoCommandHandler(
        ICarrierQueryRepository carrierQueryRepo,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<AddCarrierAdditionalInfoCommandHandler> logger)
        : ICommandHandler<AddingCarrierAdditionalInfoCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(AddingCarrierAdditionalInfoCommand request, CancellationToken ct)
        {
            //===== Retrieve Carrier  Attempts ==========
            var carrier = await carrierQueryRepo.GetCarrierAsync(c => c.AppUserId == request.UserId, ct);
            if (carrier is null)
            {
                logger.LogWarning("Carrier additional information update rejected because Carrier {CarrierId} was not found", request.UserId);
                return BaseResult.NotFound(Errors.CarrierNotFound("Carrier Not Found"));
            }

            //====== Adding Carrier Info =====
            carrier.AddAdditionalData(
                request.UserId,
                request.Dto.LicenseNumber,
                request.Dto.MaxDailyShipments,
                request.Dto.DefaultCostPerKg,
                request.Dto.YearsOfExperience,
                request.Dto.ContractStartDate,
                request.Dto.ContractYears,
                request.Dto.WarehouseId);

            //==== Saving Changes =========
            
            await unitOfWork.SaveChangesAsync(ct);
          

            logger.LogInformation("Carrier additional information saved successfully for Carrier {CarrierId}", request.UserId);
            await cacheService.RemoveAsync(CacheKeys.CarrierProfile(carrier.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(carrier.Id));
            return BaseResult.Success();
        }
    }
}
