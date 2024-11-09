namespace Leoxia.Scheduling.Abstractions
{
    public interface IHistorizedJobRepository
    {
        void Add(IHistorizedJobRun historizedRun);
        IEnumerable<IHistorizedJobRun> GetJobRuns();
    }

    public interface IHistorizedJobRun
    {
        IJob Job { get; }
        DateTimeOffset Start { get; }
        DateTimeOffset? End { get; }
        bool IsRunning { get; }
    }
}
