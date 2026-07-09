using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace TransitNova.InfraStructure.ServiceRegistration.HangFireServiceRegisteration
{
    public static class HangRegisterationExtension
    {
        public static void AddHangFireService(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                })
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();

            });
            services.AddHangfireServer();
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount * 5;
                options.Queues = ["critical", "default", "reports"];
            });
        }

    }
}
