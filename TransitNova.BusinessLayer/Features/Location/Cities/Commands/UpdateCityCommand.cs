using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Commands
{
    public sealed record UpdateCityCommand(Guid RequestId, UpdateCityDto Dto) : IdempotantCommand<BaseResult>(RequestId);
}
