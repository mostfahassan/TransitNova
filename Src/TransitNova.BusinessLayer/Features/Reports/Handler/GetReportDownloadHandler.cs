using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Features.Reports.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
namespace TransitNova.BusinessLayer.Features.Reports.Handler
{
    public sealed class GetReportDownloadHandler(
        IReportRequestQueryRepository reportRequestQueryRepository,
        ILogger<GetReportDownloadHandler> logger)
        : IQueryHandler<GetReportDownloadQuery, Result<ReportDownloadDto>>
    {
        public async Task<Result<ReportDownloadDto>> Handle(GetReportDownloadQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving report download for ReportId {ReportId} and User {UserId}.", request.ReportId, request.RequestedBy);

            var download = await reportRequestQueryRepository.GetReportDownloadAsync(request.ReportId, request.RequestedBy, request.CanAccessAll, cancellationToken);

            if (download is null)
            {
                logger.LogWarning("Report download was not found or is not accessible. ReportId: {ReportId}, UserId: {UserId}", request.ReportId, request.RequestedBy);

                return Result<ReportDownloadDto>.NotFound(Errors.NotFound("Report was not found, is not ready yet, or you do not have access to it."));
            }

            return Result<ReportDownloadDto>.Success(download);
        }
    }
}
