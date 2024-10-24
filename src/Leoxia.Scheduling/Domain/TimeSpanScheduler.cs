namespace Leoxia.Scheduling.Domain;

internal class TimeSpanScheduler : IRunScheduler
{
    private readonly TimeSpan _timeSpan;

    public TimeSpanScheduler(TimeSpan timeSpan)
    {
        _timeSpan = timeSpan;
    }

    public DateTimeOffset? GetNextRun(DateTimeOffset now)
    {
        return now + _timeSpan;
    }
}