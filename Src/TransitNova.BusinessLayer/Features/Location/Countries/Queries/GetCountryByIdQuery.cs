using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Queries
{
    public sealed record GetCountryByIdQuery(int Id) : IQuery<Result<CountryDto?>>;
}

