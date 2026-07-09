using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Reports.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Reports.Handler
{
    public sealed class GenerateReportHandler(
        IReportRequestCommands reportRequestCommands,
        IUnitOfWork unitOfWork,
        ILogger<GenerateReportHandler> logger)
        : ICommandHandler<GenerateReportCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
        {
            var parameters = request.Dto.Parameters ?? new Dictionary<string, string>();
            var reportRequest = ReportRequest.CreateReport(
                request.Dto.ReportType,
                parameters,
                request.Dto.RequestedBy);

            await reportRequestCommands.AddReportRequstAsync(reportRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Report request {ReportId} created by User {UserId} for {ReportType}.",
                reportRequest.Id,
                request.Dto.RequestedBy,
                request.Dto.ReportType);

            return BaseResult.Created("Report request created successfully.");
        }
    }
}
