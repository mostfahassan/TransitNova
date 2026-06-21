using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record AddingCarrierAdditionalInfoCommand(Guid RequestId, AdditionalInfoDto Dto, Guid UserId) : IdempotantCommand<BaseResult>(RequestId), ITransactional;
}
