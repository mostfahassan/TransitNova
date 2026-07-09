using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.Api.Contracts.Reports;
using TransitNova.Api.Infrastructure.Idempotency;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.BusinessLayer.Features.Reports.Commands;
using TransitNova.BusinessLayer.Features.Reports.Queries;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Reports
{
    [Authorize(Roles = Role.AllUsers)]
    [Route("api/v{version:apiVersion}/reports")]
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Reports")]
    public sealed class ReportsController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("request")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Request Report")]
        [EndpointSummary("Request a new report")]
        [EndpointDescription("Allows an authorized user to request a new report with specified parameters. This endpoint is idempotent and should be called with X-Idempotency-Key.")]
        public async Task<IActionResult> RequestReportAsync([IdempotencyKey] Guid requestId, [FromBody] CreateReportRequest request, CancellationToken cancellationToken)
        {
            var reportRequest = new ReportCommand
            {
                ReportType = request.ReportType,
                Parameters = request.Parameters ?? new Dictionary<string, string>(),
                RequestedBy = User.GetUserId()
            };

            var response = await mediator.Send(new GenerateReportCommand(requestId, reportRequest), cancellationToken);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{reportId:guid}/download")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Download Report")]
        [EndpointSummary("Download a generated report")]
        [EndpointDescription("Allows an authorized user to download a generated report by its ID. Admins can download any generated report.")]
        public async Task<IActionResult> DownloadReportAsync(Guid reportId, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(
                new GetReportDownloadQuery(reportId, User.GetUserId(), User.IsInRole(Role.Admin)),
                cancellationToken);

            if (response.IsFailure || response.Data is null)
            {
                return response.ToActionResult();
            }

            if (!System.IO.File.Exists(response.Data.FilePath))
            {
                return Result<ReportDownloadDto>.NotFound(
                    Errors.NotFound("Generated report file was not found on disk."))
                    .ToActionResult();
            }

            return PhysicalFile(response.Data.FilePath, "application/octet-stream", response.Data.FileName);
        }
    }
}
