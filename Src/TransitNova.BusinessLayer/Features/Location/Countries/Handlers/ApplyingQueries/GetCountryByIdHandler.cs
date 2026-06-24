using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyingQueries
{
    public sealed class GetCountryByIdHandler(ICountryRepository repository)
        : IQueryHandler<GetCountryByIdQuery, Result<CountryDto?>>
    {
        public async Task<Result<CountryDto?>> Handle(GetCountryByIdQuery request, CancellationToken ct)
        {
            var dto = await repository.GetByIdAsync<CountryDto>(request.Id, ct);
            return dto is null ? Result<CountryDto?>.NotFound(Errors.NotFound("Country not found.")) : Result<CountryDto?>.Success(dto);
        }
    }
}

