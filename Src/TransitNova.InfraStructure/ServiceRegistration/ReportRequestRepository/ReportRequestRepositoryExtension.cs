using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.ReportsRepository;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
using TransitNova.InfraStructure.Common.Interfaces.Implementation;
using TransitNova.InfraStructure.Repository.Reports;

namespace TransitNova.InfraStructure.ServiceRegistration.ReportRequestRepository
{
    public static class ReportRequestRepositoryExtension
    {
        public static void AddReportRequestRepository(this IServiceCollection services)
        {
            services.AddScoped<IReportRequestCommands, ReportRequestCommands>();
            services.AddScoped<IReportRequestQueryRepository, ReportRequestQueryRepository>();
            services.AddScoped<IFileStorageService, FileStorageService>();
        }
    }
}
