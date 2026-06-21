using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands
{
    public sealed class CreateCityHandler(
        ICityRepository repository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateCityCommand, Result<CityDto>>
    {
        public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken ct)
        {
            var city =  City.Create(request.Dto.Name.Trim(), request.Dto.GovernmentId);
            await repository.AddAsync(city, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await cacheService.RemoveAsync(CacheKeys.CitiesByGovernment(request.Dto.GovernmentId));

            // Map the created city to CityDto
            var map = new CityDto
            {      
                Name = city.Name,
                GovernmentName = city.Government.Name
            };    
            
            return Result<CityDto>.Success(map);
        }
    }
}
