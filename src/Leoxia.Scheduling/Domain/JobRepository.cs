using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobRepository : IJobRepository
{
    private readonly ILogger<JobRepository> _logger;
    private readonly ConcurrentBag<Job> _jobs = new ();

    public JobRepository(
        ILogger<JobRepository> logger)
    {
        _logger = logger;
    }

    public void Add(Job job)
    {
        _logger.LogInformation($"Job {job.Type} scheduled.");
        _jobs.Add(job);
    }

    public IEnumerable<IJob> GetJobs()
    {
        return _jobs.ToArray().Cast<IJob>();
    }
}

internal class JobRunRepository : IJobRunRepository
{
    private readonly ConcurrentDictionary<Job, JobRun> _runs = new();

    public IEnumerable<JobRun> GetJobRuns()
    {
        return _runs.Values.ToArray();
    }

    public JobRun GetOrAdd(IJob job)
    {
        return _runs.GetOrAdd((Job)job, j => new JobRun(j, j.CreationDate));
    }
}