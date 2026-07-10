using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Reports;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Features.Reports.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Reports.Handler
{
    public sealed class GenerateDashboardReportHandler(
        IReportRequestCommands reportRequestCommands,
        IUnitOfWork unitOfWork,
        ILogger<GenerateDashboardReportHandler> logger)
        : ICommandHandler<GenerateDashboardReportCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(GenerateDashboardReportCommand request, CancellationToken cancellationToken)
        {
            var payloadJson = ReportPayloadSerializer.Serialize(request.Contract);
            var reportRequest = ReportRequest.CreateReport(DashboardReportContract.ReportKey, payloadJson, request.RequestedBy);

            await reportRequestCommands.AddReportRequestAsync(reportRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Report request {ReportId} created by User {UserId} for {ReportKey}.",
                reportRequest.Id,
                request.RequestedBy,
                DashboardReportContract.ReportKey);

            return BaseResult.Created("Dashboard report request created successfully.");
        }
    }
}
