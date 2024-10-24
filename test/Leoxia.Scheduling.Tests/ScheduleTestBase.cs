using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Extensions.Logging;
using NUnit.Framework;
using System.Globalization;

namespace Leoxia.Scheduling.Tests;

public class ScheduleTestBase
{
    private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly ServiceProvider _serviceProvider;
    private readonly IStoppable _jobTimer;
    private readonly MockTimerFactory _timerFactory = new();
    private readonly MockTimeProvider _timeProvider = new();

    public ScheduleTestBase(Action<IServiceCollection>? configureServices = null)
    {
        _logger.Info("Scheduler Test created");
        var loggerFactory = new NLogLoggerFactory();
        var services = new ServiceCollection();
        services.AddSingleton<ITimeProvider>(_timeProvider);
        services.AddSingleton<ITimerFactory>(_timerFactory);
        services.AddSingleton<MyJob>();
        configureServices?.Invoke(services);
        services.AddJobScheduler();
        _serviceProvider = services.BuildServiceProvider();
        _jobTimer = _serviceProvider.UseJobScheduler(_ => { })
            .WithTimerPeriod(TimeSpan.FromMilliseconds(100))
            .WithLoggerFactory(loggerFactory)
            .LogExceptions()
            .Start()
            .Result;
    }

    protected MockTimeProvider Time => _timeProvider;

    protected MockTimerFactory Timer => _timerFactory;

    protected IJobScheduler BuildScheduler()
    {
        return Resolve<IJobScheduler>();
    }

    protected T Resolve<T>() where T: class
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    protected void IncreaseTime(TimeSpan span)
    {
        Time.Set(Time.UtcNow() + span);
    }

    protected void RaiseNextTick()
    {
        Time.Set(Time.UtcNow() + TimeSpan.FromMilliseconds(1001));
        Timer.Tick();
    }

    protected DateTimeOffset ParseTime(string time)
    {
        time += "+00:00";
        var offset = DateTimeOffset.ParseExact(time, "yyyy/MM/dd HH:mm:sszzz", null);
        return offset;
    }

    [TearDown]
    public async Task TestTearDown()
    {
        await _jobTimer.Stop();
        await _serviceProvider.DisposeAsync();
    }
}