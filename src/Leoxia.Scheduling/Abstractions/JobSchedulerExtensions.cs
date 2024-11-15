﻿namespace Leoxia.Scheduling.Abstractions;

public static class JobSchedulerExtensions
{
    public static IJobBuilder Schedule<T>(this IJobScheduler scheduler) where T : IInvocable
    {
        return scheduler.Schedule(typeof(T));
    }

    public static IJobBuilder Schedule(this IJobScheduler scheduler, Func<Task> asyncTaskProvider)
    {
        return scheduler.Schedule(new JobTaskWrapper(asyncTaskProvider));
    }

    public static IJobBuilder Schedule(this IJobScheduler scheduler, Action action)
    {
        return scheduler.Schedule(new JobWrapper(action));
    }

    public static IJobBuilder ThenRun(this IJobBuilder builder, Action<IInvocable> action)
    {
        return builder.ThenRun(Wraps(action));
    }

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

    private static Func<IInvocable, Task> Wraps(Action<IInvocable> action)
    {
        return j =>
        {
            action(j);
            return Task.CompletedTask;
        };
    }
}

public class JobTaskWrapper : IInvocable
{
    private readonly Func<Task> _asyncTaskProvider;

    public JobTaskWrapper(Func<Task> asyncTaskProvider)
    {
        _asyncTaskProvider = asyncTaskProvider;
    }

    public async Task Invoke()
    {
        await _asyncTaskProvider();
    }
}

public class JobWrapper : IInvocable
{
    private readonly Action _action;

    public JobWrapper(Action action)
    {
        _action = action;
    }

    public Task Invoke()
    {
        _action();
        return Task.CompletedTask;
    }
}