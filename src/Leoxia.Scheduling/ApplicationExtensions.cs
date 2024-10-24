using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling
{
    public static class ApplicationExtensions
    {
        public static IJobSchedulerConfiguration UseJobScheduler(
            this IServiceProvider services,
            Action<IJobScheduler> schedulerConfigure)
        {
            var jobScheduler = services.GetRequiredService<IJobScheduler>();
            schedulerConfigure(jobScheduler);
            return services.GetRequiredService<IJobSchedulerConfiguration>();
        }
    }
}