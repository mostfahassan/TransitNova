using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TransitNova.InfraStructure.BackgroundJobs;
namespace TransitNova.InfraStructure.ServiceRegistration.BackgroundJobsRegistration
{
    public static class BackgroundJobsRegistrationExtension
    {

        public static IServiceCollection AddBackgroundJobsServices(this IServiceCollection services )
        {
            services.AddQuartz(opt =>
            {
                var JobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
                opt.AddJob<ProcessOutboxMessagesJob>(JobKey)
                .AddTrigger(trigger => trigger.ForJob(JobKey)
                                .WithIdentity($"{nameof(ProcessOutboxMessagesJob)}-trigger")
                                            .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()
                                                          .WithMisfireHandlingInstructionIgnoreMisfires()));
            });
            services.AddQuartzHostedService();

            return services;
        }
    }
}
