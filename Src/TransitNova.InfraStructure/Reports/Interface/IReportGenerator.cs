namespace TransitNova.InfraStructure.Reports.Interface
{
    public interface IReportGenerator
    {
        string ReportKey { get; }
        Task<string> GenerateReportAsync( string payloadJson, CancellationToken cancellationToken , Guid? reportId = null);
    }
}
