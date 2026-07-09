using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;
namespace TransitNova.BusinessLayer.Features.Reports.Commands
{
    public record GenerateReportCommand(Guid RequestId, ReportCommand Dto):
        IdempotentCommand<BaseResult>(RequestId);
}
