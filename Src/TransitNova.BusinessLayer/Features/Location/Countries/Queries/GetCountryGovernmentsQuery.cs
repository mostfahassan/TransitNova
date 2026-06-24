using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
namespace TransitNova.BusinessLayer.Features.Location.Countries.Queries
{
    public sealed record GetCountryGovernmentsQuery(int countryId) : IQuery<Result<IEnumerable<GovernmentDto>>>;
   
}
