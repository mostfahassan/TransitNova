using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Services.Reports;
using TransitNova.InfraStructure.BackgroundJobs;
using TransitNova.InfraStructure.Common.PdfGenerator.Helpers;
using TransitNova.InfraStructure.Reports.Interface;
using TransitNova.InfraStructure.Reports.ReportFactory;
using TransitNova.InfraStructure.Reports.Reports;

namespace TransitNova.InfraStructure.ServiceRegistration.ReportStrategyRegistration
{
    public static class ReportStrategyRegistrationExtension
    {
        public static void AddReportStrategyRegistration(this IServiceCollection services)
        {
            services.AddScoped<IReportGenerator, DashboardReportGenerator>();
            services.AddScoped<IReportGenerator, ShipmentAnalyticsReportGenerator>();
            services.AddScoped<IReportGenerator, InvoiceReportGenerator>();
            services.AddScoped<IReportGeneratorFactory, ReportGeneratorFactory>();
            services.AddScoped<IReportGenerationJob, ReportGenerationJob>();
            services.AddScoped<IPdfDocumentFactory, PdfDocumentFactory>();
        }
    }
}

