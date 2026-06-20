using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands
{
    public record DeleteCarrierCompanyCommand(Guid RequestId, Guid Id)
        : IdempotantCommand<BaseResult>(RequestId);

}
