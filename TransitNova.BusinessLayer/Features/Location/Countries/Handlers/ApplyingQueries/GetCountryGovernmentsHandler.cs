using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyingQueries
{
    public sealed class GetCountryGovernmentsHandler(ICountryRepository repo) : IQueryHandler<GetCountryGovernmentsQuery, Result<IEnumerable<GovernmentDto>>>
    {
        public async Task<Result<IEnumerable<GovernmentDto>>> Handle(GetCountryGovernmentsQuery request, CancellationToken cancellationToken)
        {
            var governments = await repo.GetAllGovernments(request.countryId, cancellationToken);
            if (!governments.Any())
            {
                return Result<IEnumerable<GovernmentDto>>.Success([]);
            }
            return Result<IEnumerable<GovernmentDto>>.Success(governments);
        }
    }
}
