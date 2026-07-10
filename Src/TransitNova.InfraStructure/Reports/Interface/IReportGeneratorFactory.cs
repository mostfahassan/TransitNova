namespace TransitNova.InfraStructure.Reports.Interface
{
    public interface IReportGeneratorFactory
    {
        IReportGenerator? ReportGeneratorResolver(string reportKey);
    }
}
