using TransitNova.InfraStructure.Reports.Interface;

namespace TransitNova.InfraStructure.Reports.ReportFactory
{
    public sealed class ReportGeneratorFactory : IReportGeneratorFactory
    {
        private readonly IReadOnlyDictionary<string, IReportGenerator> _reportGenerators;

        public ReportGeneratorFactory(IEnumerable<IReportGenerator> reportGenerators)
        {
            _reportGenerators = reportGenerators.ToDictionary(
                generator => generator.ReportKey,
                StringComparer.OrdinalIgnoreCase);
        }

        public IReportGenerator? ReportGeneratorResolver(string reportKey)
        {
            if (string.IsNullOrWhiteSpace(reportKey))
            {
                return null;
            }

            _reportGenerators.TryGetValue(reportKey, out var generator);
            return generator;
        }
    }
}
