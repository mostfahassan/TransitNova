using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands
{
    public sealed record UpdateCountryCommand(Guid RequestId, UpdateCountryDto Dto) : IdempotentCommand<BaseResult>(RequestId);
}
