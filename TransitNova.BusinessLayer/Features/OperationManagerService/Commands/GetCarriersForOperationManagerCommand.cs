using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public record GetCarriersForOperationManagerCommand(FilterCarrierDto FilterCriteria) : ICommand<Result<PagedResult<CarrierSummaryDetailsDto>>>;
}
