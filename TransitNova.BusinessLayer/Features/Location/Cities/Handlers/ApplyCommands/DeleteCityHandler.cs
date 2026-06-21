using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class DeleteCityHandler(ICityRepository repository, IUnitOfWork unitOfWork, ICacheService cacheService,ILogger<DeleteCityHandler>logger)
        : ICommandHandler<DeleteCityCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteCityCommand request, CancellationToken ct)
        {
            var city = await repository.GetByIdAsync<City>(request.Id, ct);
            if (city == null)
                return BaseResult.NotFound(Errors.NotFound("City Not Found"));


            var deleted = await repository.DeleteAsync(request.Id, ct);
            if (!deleted)
            {
                logger.LogWarning("City delete failed because City was not found.CityId: {CityId}", request.Id);
                return BaseResult.UnExpected(Errors.FailedOperation("Un Error Occured While Removing City"));
            }

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
