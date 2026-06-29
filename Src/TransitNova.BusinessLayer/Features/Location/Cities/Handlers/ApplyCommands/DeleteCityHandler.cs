using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class DeleteCityHandler(ICityRepository repository, IUnitOfWork unitOfWork,ILogger<DeleteCityHandler>logger)
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
                return BaseResult.UnExpected(Errors.FailedOperation("An error occurred while removing the city."));
            }

            await unitOfWork.SaveChangesAsync(ct);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Cities.ById(request.Id),
                city is not null ? CacheKeys.Cities.ByGovernment(city.GovernmentId) : string.Empty);
            return BaseResult.Success();
        }
    }
}


