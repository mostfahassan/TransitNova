using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
 
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class DeleteCityHandler(ICityRepository repository, IUnitOfWork unitOfWork, ICacheService cacheService)
        : ICommandHandler<DeleteCityCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteCityCommand request, CancellationToken ct)
        {
            var city = await repository.GetByIdAsync<City>(request.Id, ct);
            await repository.DeleteAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await cacheService.RemoveAsync(CacheKeys.CityById(request.Id));
            if (city is not null)
            {
                await cacheService.RemoveAsync(CacheKeys.CitiesByGovernment(city.GovernmentId));
            }
            return BaseResult.Success();
        }
    }
}
