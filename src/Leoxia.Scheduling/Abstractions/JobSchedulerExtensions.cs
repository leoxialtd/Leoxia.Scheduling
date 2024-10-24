namespace Leoxia.Scheduling.Abstractions;

public static class JobSchedulerExtensions
{
    public static IJobBuilder Schedule<T>(this IJobScheduler scheduler) where T : IInvocable
    {
        return scheduler.Schedule(typeof(T));
    }

    // TODO: fix this to support current usage
    //public static IJobBuilder Schedule(this IJobScheduler scheduler, Func<Task> asyncTaskProvider)
    //{
    //    return scheduler.Schedule();
    //}

    //public static IJobBuilder Schedule(this IJobScheduler scheduler, Action actionToSchedule)
    //{
    //    return scheduler.Schedule();
    //}

    public static IJobBuilder EverySeconds(this IJobBuilder builder, int seconds = 1)
    {
        return builder.Every(TimeSpan.FromSeconds(seconds));
    }

    public static IJobBuilder EveryMinutes(this IJobBuilder builder, int minutes = 1)
    {
        return builder.Every(TimeSpan.FromMinutes(minutes));
    }

    public static IJobBuilder Hourly(this IJobBuilder builder, int hour = 1)
    {
        return builder.Every(TimeSpan.FromHours(hour));
    }


    public static IJobBuilder Daily(this IJobBuilder builder, int days = 1)
    {
        return builder.Every(TimeSpan.FromDays(days));
    }

    public static IJobBuilder Monthly(this IJobBuilder builder, int months = 1)
    {
        return builder.Every(TimeSpan.FromDays(months));
    }

    public static IJobBuilder Once(this IJobBuilder builder)
    {
        return builder.Times(1);
    }
}