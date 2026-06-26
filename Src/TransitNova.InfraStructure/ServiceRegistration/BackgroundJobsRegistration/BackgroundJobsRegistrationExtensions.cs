using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TransitNova.InfraStructure.BackgroundJobs;
namespace TransitNova.InfraStructure.ServiceRegistration.BackgroundJobsRegistration
{
    public static class BackgroundJobsRegistrationExtensions
    {

        public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
                options.AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                                .WithIdentity($"{nameof(ProcessOutboxMessagesJob)}-trigger")
                                            .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(5).RepeatForever()
                                                          .WithMisfireHandlingInstructionIgnoreMisfires()));
            });
            services.AddQuartzHostedService();

            return services;
        }
    }
}
