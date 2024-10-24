using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobSchedulerConfiguration : IJobSchedulerConfiguration
{
    private readonly IServiceProvider _provider;
    private readonly ConfigurableLoggerFactory _loggerFactory;
    private ILogger<IJobScheduler> _logger;

    public JobSchedulerConfiguration(IServiceProvider provider, ConfigurableLoggerFactory loggerFactory)
    {
        _provider = provider;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<IJobScheduler>();
        ExceptionHandler = OnException;
    }

    private void OnException(Exception exception, string context)
    {
        // Do nothing by default
    }

    public TimeSpan TimerPeriod { get; private set; } = TimeSpan.FromSeconds(1);

    public Action<Exception, string> ExceptionHandler { get; private set; }

    public IJobSchedulerConfiguration WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory.SetLoggerFactory(loggerFactory);
        _logger = _loggerFactory.CreateLogger<IJobScheduler>();
        return this;
    }

    public IJobSchedulerConfiguration OnException(Action<Exception, string> onException)
    {
        ExceptionHandler = onException;
        return this;
    }

    public IJobSchedulerConfiguration LogExceptions()
    {
        ExceptionHandler = LogException;
        return this;
    }

    public IJobSchedulerConfiguration WithTimerPeriod(TimeSpan period)
    {
        TimerPeriod = period;
        return this;
    }

    private void LogException(Exception exception, string context)
    {
        _logger.LogError(exception, context);
    }

    public async Task<IStoppable> Start()
    {
        var timer = _provider.GetRequiredService<JobTimer>();
        await timer.Start();
        return new StoppableTimer(timer);
    }
}

internal class StoppableTimer : IStoppable
{
    private readonly JobTimer _timer;

    public StoppableTimer(JobTimer timer)
    {
        _timer = timer;
    }

    public async Task Stop()
    {
        await _timer.Stop();
    }
}