using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record UpdateCarrierProfileCommand(Guid RequestId, Guid AppUserId, UpdateCarrierDto Dto)
        : IdempotentCommand<Result<CarrierProfileDto>>(RequestId), ITransactional, ICacheInvalidator;
}


