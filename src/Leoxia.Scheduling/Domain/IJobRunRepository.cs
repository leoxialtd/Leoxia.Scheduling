using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal interface IJobRunRepository
{
    IEnumerable<JobRun> GetJobRuns();
    JobRun GetOrAdd(IJob job);
}