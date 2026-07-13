using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Features.Reports.Commands
{
    public sealed record GenerateBundleReportCommand(Guid RequestId, BundleReportContract Contract, Guid RequestedBy)
        : IdempotentCommand<BaseResult>(RequestId);
}
