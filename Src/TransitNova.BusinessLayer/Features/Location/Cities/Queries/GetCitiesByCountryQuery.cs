using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Queries
{
    public sealed record GetCitiesByCountryQuery(int GovernmentId) : IQuery<Result<IEnumerable<CityDto>>>;
}

