using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobEngine
{
    private readonly ILogger<JobEngine> _logger;
    private readonly IJobRunRepository _repository;
    private readonly JobSchedulerConfiguration _configuration;
    private readonly IFastTimeProvider _timeProvider;
    private readonly ConcurrentDictionary<int, Task> _runningTasks = new ();
    private readonly object _synchro = new object();

    private volatile bool _isRunning = true;

    public JobEngine(
        ILogger<JobEngine> logger,
        IJobRunRepository repository,
        JobSchedulerConfiguration configuration,
        IFastTimeProvider timeProvider)
    {
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    public void Run(DateTimeOffset now)
    {
        var scheduledRuns = new List<JobRun>();
        var runs = _repository.GetJobRuns();
        lock (_synchro)
        {
            foreach (var run in runs)
            {
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
            var task = Task.Run(async () =>
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
                            action(invocable.Invocable);
                        }

                        _logger.LogDebug($"[Scheduling] Run {run}. Queued actions invoked.");
                    }
                }
                catch (Exception e)
                {
                    _configuration.ExceptionHandler(e, $"While running {run}");
                }
            });
            _runningTasks.TryAdd(task.Id, task);
        }

        Task.Run(Cleanup);
    }

    private void Cleanup()
    {
        foreach (var task in _runningTasks.Values)
        {
            if (task.IsCompleted)
            {
                _runningTasks.TryRemove(task.Id, out _);
            }
        }
    }

    public async Task Stop()
    {
        _isRunning = false;
        await Task.WhenAll(_runningTasks.Values);
    }
}