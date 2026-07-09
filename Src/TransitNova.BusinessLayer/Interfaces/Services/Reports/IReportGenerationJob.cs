namespace TransitNova.BusinessLayer.Interfaces.Services.Reports
{
    public interface IReportGenerationJob
    {
        Task GenerateAsync(Guid reportRequestId, CancellationToken cancellationToken);
    }
}
