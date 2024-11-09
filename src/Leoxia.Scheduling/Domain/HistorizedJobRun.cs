using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class HistorizedJobRun : IHistorizedJobRun
{
    public HistorizedJobRun(JobRun jobRun)
    {
        this.Job = jobRun.Job;
        Start = jobRun.Start;
    }

    public DateTimeOffset Start { get; private set; }
    public DateTimeOffset? End { get; private set; }
    public bool IsRunning => End == null;
    public IJob Job { get; private set; }

    internal void SetEnd(DateTimeOffset utcNow)
    {
        End = utcNow;
    }

    public override string ToString()
    {
        return $"[{Job.Name}:{Start}-{End}]";
    }
}