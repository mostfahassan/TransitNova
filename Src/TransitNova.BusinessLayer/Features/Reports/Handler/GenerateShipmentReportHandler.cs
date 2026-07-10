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
    public sealed class GenerateShipmentReportHandler(
        IReportRequestCommands reportRequestCommands,
        IUnitOfWork unitOfWork,
        ILogger<GenerateShipmentReportHandler> logger)
        : ICommandHandler<GenerateShipmentReportCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(GenerateShipmentReportCommand request, CancellationToken cancellationToken)
        {
            var payloadJson = ReportPayloadSerializer.Serialize(request.Contract);
            var reportRequest = ReportRequest.CreateReport(ShipmentReportContract.ReportKey, payloadJson, request.RequestedBy);

            await reportRequestCommands.AddReportRequestAsync(reportRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Report request {ReportId} created by User {UserId} for {ReportKey}.",
                reportRequest.Id,
                request.RequestedBy,
                ShipmentReportContract.ReportKey);

            return BaseResult.Created("Shipment report request created successfully.");
        }
    }
}
