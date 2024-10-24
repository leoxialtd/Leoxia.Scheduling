using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobRepository : IJobRepository, IJobRunRepository
{
    private readonly IFastTimeProvider _provider;
    private readonly ILogger<JobRepository> _logger;
    private readonly ConcurrentBag<Job> _jobs = new ConcurrentBag<Job>();
    private readonly ConcurrentBag<JobRun> _runs = new ();

    public JobRepository(
        IFastTimeProvider provider,
        ILogger<JobRepository> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public void Add(Job job)
    {
        _logger.LogInformation($"Job {job.Type} scheduled.");
        _jobs.Add(job);
        var now = _provider.UtcNow();
        _runs.Add(new JobRun(job, now));
    }

    public IEnumerable<IJob> GetJobs()
    {
        return _jobs.ToArray().Cast<IJob>();
    }

    public IEnumerable<JobRun> GetJobRuns()
    {
        return _runs.ToArray();
    }
}