using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.Bundles.Commands
{
    public sealed record CreateBundleCommand(Guid RequestId, Guid UserId, CreateBundleDto Dto)
      : IdempotantCommand<BaseResult>(RequestId),ITransactional;
}
