using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class UpdateCityHandler(
        ICityRepository repository,
        ILogger<UpdateCityHandler> logger,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateCityCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCityCommand request, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync<City>(request.CityId, ct);
            if (entity == null)
            {
                logger.LogInformation("No City Found With Id {CityId}", request.CityId);
                throw new EntityNotFoundException($"City {request.CityId} was not found.", "CITY_NOT_FOUNDED", nameof(City));
            }
            var oldGovernmentId = entity.GovernmentId;
            entity.Update(request.Dto.Name.Trim(), request.Dto.GovernmentId);
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Cities.ById(request.CityId),
                CacheKeys.Cities.ByGovernment(oldGovernmentId),
                CacheKeys.Cities.ByGovernment(request.Dto.GovernmentId));
            return BaseResult.Success();
        }
    }
}


