using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands
{
    public sealed class UpdateCarrierProfileHandler(
        ICarrierQueryRepository carrierRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<UpdateCarrierProfileHandler> logger)
        : ICommandHandler<UpdateCarrierProfileCommand, Result<CarrierProfileDto>>,ITransactional
    {
        public async Task<Result<CarrierProfileDto>> Handle(UpdateCarrierProfileCommand request, CancellationToken ct)
        {
            //====== Retrieve Carrier Attempts
            var carrier = await carrierRepository.GetCarrierAsync(c => c.AppUserId == request.AppUserId, ct);
            if (carrier is null)
            {
                return Result<CarrierProfileDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found."));
            }

            //======== Update Carrier 
            carrier.UpdateProfile(request.AppUserId, request.Dto.FirstName, request.Dto.LastName, request.Dto.PhoneNumber, request.Dto.Address);

            //====== Apply Changes 
           
            await unitOfWork.SaveChangesAsync(ct);
         

            logger.LogInformation("Carrier With {UserId} Updated His Profile Successfully", carrier.Id);
            await cacheService.RemoveAsync(CacheKeys.CarrierProfile(carrier.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(carrier.Id));
            var carrierDto = CarrierProfileBuilder.FromCarrier(carrier);
            return Result<CarrierProfileDto>.Success(carrierDto);
        }
    }
}
