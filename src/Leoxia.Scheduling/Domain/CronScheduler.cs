using Leoxia.Scheduling.Domain.Cron;

namespace Leoxia.Scheduling.Domain;

internal class CronScheduler : IRunScheduler
{
    private readonly CronExpression _cronExpression;

    public CronScheduler(string cronExpression)
    {
        _cronExpression = new CronExpression(cronExpression);
    }

    public DateTimeOffset? GetNextRun(DateTimeOffset now)
    {
        return _cronExpression.GetNextTrigger(now);
    }
}