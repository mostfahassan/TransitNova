using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class UpdateCityHandler(
        ICityRepository repository,
        ILogger<UpdateCityHandler> logger,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateCityCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCityCommand request, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync<City>(request.Dto.Id, ct);
            if (entity == null)
            {
                logger.LogInformation("No City Found With Id {CountryId}", request.Dto.Id);
                throw new EntityNotFoundException($"No City Found With ID:{request.Dto.Id} Has Been Founded", "CITY_NOT_FOUNDED", nameof(City));
            }
            var oldGovernmentId = entity.GovernmentId;
            entity.Update(request.Dto.Name.Trim(), request.Dto.GovernmentId);
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);
            await cacheService.RemoveAsync(CacheKeys.CityById(request.Dto.Id));
            await cacheService.RemoveAsync(CacheKeys.CitiesByGovernment(oldGovernmentId));
            await cacheService.RemoveAsync(CacheKeys.CitiesByGovernment(request.Dto.GovernmentId));
            return BaseResult.Success();
        }
    }
}
