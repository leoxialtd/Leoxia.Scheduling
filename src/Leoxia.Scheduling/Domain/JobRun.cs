using System.Collections.Concurrent;

namespace Leoxia.Scheduling.Domain;

internal class JobRun
{
    private readonly object _sync = new ();

    private DateTimeOffset? _nextRun;
    private bool _isRunning;
    public DateTimeOffset Start { get; private set; }

    public JobRun(Job job, DateTimeOffset now)
    {
        Job = job;
        _nextRun = job.RunScheduler.GetNextRun(now);
    }

    public bool ShouldRun(DateTimeOffset now)
    {
        if (HasReachMaximumExecution)
        {
            return false;
        }

        if (IsRunning && !Job.Overlapping)
        {
            return false;
        }

        var shouldRun = _nextRun != null && now >= _nextRun;
        if (shouldRun && Job.MaxRuns != null)
        {
            CurrentRun += 1;
            if (CurrentRun >= Job.MaxRuns.Value)
            {
                HasReachMaximumExecution = true;
            }
        }

        return shouldRun;
    }

    private bool HasReachMaximumExecution { get; set; }

    private int CurrentRun { get; set; }

    public Job Job { get; }

    public bool IsRunning
    {
        get
        {
            lock (_sync)
            {
                return _isRunning;
            }
        }
        set
        {
            lock (_sync)
            {
                _isRunning = value;
            }
        }
    }

    public ConcurrentBag<Task> Runs { get; } = new ConcurrentBag<Task>();

    public override string ToString()
    {
        return Job.Name;
    }

    public void SetNextRun(DateTimeOffset now)
    {
        IsRunning = true;
        Start = now;
        _nextRun = Job.RunScheduler.GetNextRun(_nextRun ?? now);
    }

    public HistorizedJobRun ToHistorizedJobRun()
    {
        return new HistorizedJobRun(this);
    }
}