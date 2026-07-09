using TransitNova.Domain.Enums.Reports;
using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Reports.ReportFactory
{
    public sealed class ReportGeneratorFactory : IReportGeneratorFactory
    {
        private readonly IEnumerable<IReportGenerator> _reportGenerators;
        public ReportGeneratorFactory(IEnumerable<IReportGenerator> reportGenerators)
            => _reportGenerators = reportGenerators;

        public IReportGenerator? ReportGeneratorResolver(ReportType reportType)
        => _reportGenerators.FirstOrDefault(generator => generator.ReportType == reportType);   
    }
}

