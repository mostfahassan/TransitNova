using TransitNova.Domain.Enums.Reports;

namespace TransitNova.InfraStructure.Reports.Interface
{
    public interface IReportGeneratorFactory
    {
        IReportGenerator? ReportGeneratorResolver(ReportType reportType);
    }
}
