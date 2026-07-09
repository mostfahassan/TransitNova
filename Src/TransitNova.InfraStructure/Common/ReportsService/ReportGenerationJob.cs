using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Services.Reports;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Common.ReportsService
{
    internal sealed class ReportGenerationJob(IReportGeneratorFactory reportGeneratorFactory, AppDbContext context) : IReportGenerationJob
    {
        public async Task GenerateAsync(Guid reportRequestId, CancellationToken cancellationToken)
        {
            var request = await context.ReportRequests.FirstOrDefaultAsync(r => r.Id == reportRequestId, cancellationToken);
            if (request is null)
            {
                return;
            }

            request.MarkAsStarted();

            var generator = reportGeneratorFactory.ReportGeneratorResolver(request.ReportType)
                ?? throw new InvalidOperationException($"No report generator is registered for report type '{request.ReportType}'.");

            var filePath = await generator.GenerateReportAsync(request.Id, request.Parameters, cancellationToken);
            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Exists ? (int)Math.Min(fileInfo.Length, int.MaxValue) : 0;

            request.MarkAsCompleted(filePath, fileSize);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
