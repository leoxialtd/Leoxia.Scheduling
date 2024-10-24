using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

/// <summary>
/// Starts and stops the timer loop.
/// </summary>
internal class JobTimer
{
    private readonly ILogger<JobTimer> _logger;
    private readonly ITimeProvider _timeProvider;
    private readonly FastTimeProvider _fastTimeProvider;
    private readonly JobEngine _engine;
    private readonly JobSchedulerConfiguration _configuration;
    private readonly ITimer _timer;
    private readonly TaskCompletionSource _runningTaskSource;

    private volatile bool _running;
    private volatile bool _starting;

    public JobTimer(
        ILogger<JobTimer> logger,
        ITimeProvider timeProvider,
        FastTimeProvider fastTimeProvider,
        JobEngine engine,
        JobSchedulerConfiguration configuration,
        ITimerFactory timerFactory)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        _fastTimeProvider = fastTimeProvider;
        _engine = engine;
        _configuration = configuration;
        _timer = timerFactory.Create(OnTimerTick);
        _runningTaskSource = new TaskCompletionSource();
    }

    private void OnTimerTick()
    {
        if (_starting)
        {
            _starting = false;
            _runningTaskSource.SetResult();
        }

        if (_running)
        {
            try
            {
                var now = _timeProvider.UtcNow();
                _fastTimeProvider.Set(now);
                _engine.Run(now);
            }
            catch (Exception e)
            {
                _configuration.ExceptionHandler(e, "On timer tick, when running engine");
            }
        }
    }

    public Task Start()
    {
        _starting = true;
        _running = true;
        _timer.Change(TimeSpan.Zero, _configuration.TimerPeriod);
        _logger.LogInformation("[Scheduling] Timer started");
        return _runningTaskSource.Task;
    }

    public async Task Stop()
    {
        _logger.LogInformation("[Scheduling] Timer stopped");
        _running = false;
        await _engine.Stop();
        await _timer.DisposeAsync();
    }
}