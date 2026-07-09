using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Reports.Commands
{
    public sealed record DownloadReportCommand(Guid reportId) : ICommand<BaseResult>;
    
}
