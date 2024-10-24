using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Domain;
using Leoxia.Scheduling.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling;

public static class ServiceExtensions
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services)
    {
        services.TryAddSingleton<ITimerFactory, TimerFactory>();
        services.TryAddSingleton<ITimeProvider, StandardTimeProvider>();
        services.AddSingleton<ConfigurableLoggerFactory>();
        services.TryAddSingleton<ILoggerFactory>(sp => sp.GetRequiredService<ConfigurableLoggerFactory>());
        services.TryAddSingleton(typeof(ILogger<>), typeof(LoggerWrapper<>));
        services.AddSingleton<JobSchedulerConfiguration>();
        services.AddSingleton<IJobSchedulerConfiguration>(sp => sp.GetRequiredService<JobSchedulerConfiguration>());
        services.AddSingleton<FastTimeProvider>();
        services.AddSingleton<IFastTimeProvider>(sp => sp.GetRequiredService<FastTimeProvider>());
        services.AddSingleton<IJobRunRepository>(sp => sp.GetRequiredService<JobRepository>());
        services.AddSingleton<JobEngine>();
        services.AddSingleton<JobTimer>();
        services.AddSingleton<JobRepository>();
        services.AddSingleton<IJobRepository>(sp => sp.GetRequiredService<JobRepository>());
        services.AddSingleton<IJobScheduler, JobSchedule>();
        services.AddHostedService<SchedulingService>();
        return services;
    }
}