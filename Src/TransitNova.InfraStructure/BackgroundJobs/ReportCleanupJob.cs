using Microsoft.Extensions.Logging;
using Quartz;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
namespace TransitNova.InfraStructure.BackgroundJobs
{
    internal class ReportCleanupJob(IReportRequestCommands reportsCommands,
        IReportRequestQueryRepository reportQuery,
        IFileStorageService fileStorage,
        ILogger<ReportCleanupJob> logger) 
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            var expiredReports = await reportQuery.ReportCleanableData(context.CancellationToken);
            if (!expiredReports.Any()) return;

            var expiredreportIds = new List<Guid>();
          
                foreach (var report in expiredReports)
                {
                    try
                    {
                        logger.LogDebug("Processing expired report: {ReportId}", report.Id);

                        if (await fileStorage.DeleteFileAsync(report.FilePath, context.CancellationToken))
                        {
                            expiredreportIds.Add(report.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while processing file for report: {ReportId}", report.Id);
                    }
                }
            if (expiredreportIds.Count > 0)
            {
                try
                {
                
                    await reportsCommands.ExpireReportsBulkAsync(expiredreportIds, context.CancellationToken);
                    logger.LogInformation("Successfully processed and expired {Count} reports.", expiredreportIds.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to bulk update expired reports in the database.");
                    throw; 
                }
            }
        }
    }
}
