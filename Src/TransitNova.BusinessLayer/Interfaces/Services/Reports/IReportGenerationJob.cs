namespace TransitNova.BusinessLayer.Interfaces.Services.Reports
{
    public interface IReportGenerationJob
    {
        Task DelegateToBackgroundAsync(Guid reportRequestId, CancellationToken cancellationToken);
        Task GenerateAsync(Guid reportRequestId, CancellationToken cancellationToken);
    }
}
