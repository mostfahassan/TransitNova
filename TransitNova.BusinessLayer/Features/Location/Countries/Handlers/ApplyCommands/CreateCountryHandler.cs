using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyCommands
{
    public sealed class CreateCountryHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork)

        : ICommandHandler<CreateCountryCommand, Result<CountryDto>>
    {
        public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken ct)
        {
            var country = Country .Create(request.Dto.Name.Trim());
            await repository.AddAsync(country, ct);
            await unitOfWork.SaveChangesAsync(ct);
           
            // After saving,map the entity to a DTO 
            var dto = new CountryDto
            {
                Id = country.Id,
                Name = country.Name
            };

            return Result<CountryDto>.Success(dto);
        }
    }
}
