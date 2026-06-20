using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands
{
    public sealed record CreateCountryCommand(Guid RequestId, CreateCountryDto Dto) : IdempotantCommand<Result<CountryDto>>(RequestId);
}
