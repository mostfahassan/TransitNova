using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Features.Reports.Queries
{
    public sealed record GetReportDownloadQuery(Guid ReportId, Guid RequestedBy, bool CanAccessAll)
        : IQuery<Result<ReportDownloadDto>>;
}
