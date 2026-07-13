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
    public class GenerateBundleReportHandler(IReportRequestCommands reportRequestCommands,
        IUnitOfWork unitOfWork,
        ILogger<GenerateBundleReportHandler> logger)
        : ICommandHandler<GenerateBundleReportCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(GenerateBundleReportCommand request, CancellationToken cancellationToken)
        {
            var payloadJson = ReportPayloadSerializer.Serialize(request.Contract);
            var reportRequest = ReportRequest.CreateReport(BundleReportContract.ReportKey, payloadJson, request.RequestedBy);

            await reportRequestCommands.AddReportRequestAsync(reportRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Report request {ReportId} created by User {UserId} for {ReportKey}.",
                reportRequest.Id,
                request.RequestedBy,
                BundleReportContract.ReportKey);

            return BaseResult.Created("Bundle report request created successfully.");
        }
    }
}
