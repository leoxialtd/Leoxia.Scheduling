namespace Leoxia.Scheduling.Domain;

internal interface IJobRunRepository
{
    IEnumerable<JobRun> GetJobRuns();
}