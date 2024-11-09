using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

/// <summary>
/// Computes whether it's time to schedule and next run date,
/// then runs the scheduled jobs.
/// </summary>
internal class JobEngine
{
    private readonly ILogger<JobEngine> _logger;
    private readonly ITaskRunner _taskRunner;
    private readonly IJobRepository _repository;
    private readonly IJobRunRepository _runRepository;
    private readonly IHistorizedJobRepository _historizedJobRepository;
    private readonly JobSchedulerConfiguration _configuration;
    private readonly IFastTimeProvider _timeProvider;
    private readonly ConcurrentDictionary<int, Task> _runningTasks = new ();
    private readonly object _synchro = new object();

    private volatile bool _isRunning = true;

    public JobEngine(
        ILogger<JobEngine> logger,
        ITaskRunner taskRunner,
        IJobRepository repository,
        IJobRunRepository runRepository,
        IHistorizedJobRepository historizedJobRepository,
        JobSchedulerConfiguration configuration,
        IFastTimeProvider timeProvider)
    {
        _logger = logger;
        _taskRunner = taskRunner;
        _repository = repository;
        _runRepository = runRepository;
        _historizedJobRepository = historizedJobRepository;
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    public void Run(DateTimeOffset now)
    {
        var scheduledRuns = new List<JobRun>();
        lock (_synchro)
        {
            foreach (var job in _repository.GetJobs())
            {
                var run = _runRepository.GetOrAdd(job);
                if (_isRunning && run.ShouldRun(now))
                {
                    run.SetNextRun(_timeProvider.UtcNow());
                    scheduledRuns.Add(run);
                }
            }
        }

        foreach (var run in scheduledRuns)
        {
            _logger.LogDebug($"Run {run} running...");
            var historizedRun = run.ToHistorizedJobRun();
            _historizedJobRepository.Add(historizedRun);
            var task = _taskRunner.Run(async () =>
            {
                try
                {
                    using (var invocable = run.Job.Resolver.Resolve(run.Job))
                    {
                        _logger.LogDebug($"[Scheduling] Run {run}. Invoking job...");
                        await invocable.Invocable.Invoke();
                        _logger.LogDebug($"[Scheduling] Run {run}. Job invoked.");

                        foreach (var action in run.Job.ExecutionQueue)
                        {
                            await action(invocable.Invocable);
                        }

                        _logger.LogDebug($"[Scheduling] Run {run}. Queued actions invoked.");
                    }
                }
                catch (Exception e)
                {
                    _configuration.ExceptionHandler(e, $"While running {run}");
                }
            }).ContinueWith(t =>
            {
                run.IsRunning = false;
                historizedRun.SetEnd(_timeProvider.UtcNow());
            });
            _runningTasks.TryAdd(task.Id, task);
        }

        _taskRunner.Run(Cleanup);
    }

    private Task Cleanup()
    {
        foreach (var task in _runningTasks.Values)
        {
            if (task.IsCompleted)
            {
                _runningTasks.TryRemove(task.Id, out _);
            }
        }

        return Task.CompletedTask;
    }

    public async Task Stop()
    {
        _isRunning = false;
        await Task.WhenAll(_runningTasks.Values);
    }
}