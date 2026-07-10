using Hangfire;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Services.Reports;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.BackgroundJobs
{
    internal sealed class ReportGenerationJob(AppDbContext context, IReportGeneratorFactory reportGeneratorFactory, IBackgroundJobClient backgroundJob) : IReportGenerationJob
    {
        public Task DelegateToBackgroundAsync(Guid reportRequestId, CancellationToken cancellationToken)
        {
            backgroundJob.Enqueue<IReportGenerationJob>(job => job.GenerateAsync(reportRequestId, CancellationToken.None));
            return Task.CompletedTask;
        }

        public async Task GenerateAsync(Guid reportRequestId, CancellationToken cancellationToken)
        {
            var request = await context.ReportRequests.FirstOrDefaultAsync(r => r.Id == reportRequestId, cancellationToken);
            if (request is null)
            {
                return;
            }

            request.MarkAsStarted();

            var generator = reportGeneratorFactory.ReportGeneratorResolver(request.ReportKey)
                ?? throw new InvalidOperationException($"No report generator is registered for report key '{request.ReportKey}'.");

            var filePath = await generator.GenerateReportAsync(request.PayloadJson, cancellationToken, request.Id);
            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Exists ? (int)Math.Min(fileInfo.Length, int.MaxValue) : 0;

            request.MarkAsCompleted(filePath, fileSize);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
