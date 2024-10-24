using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Logging;

public class LoggerWrapper<T> : ILogger<T>
{
    private readonly ConfigurableLoggerFactory _loggerFactory;
    private ILogger<T> _loggerImplementation;

    public LoggerWrapper(ConfigurableLoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _loggerImplementation = loggerFactory.CreateLogger<T>();
        loggerFactory.Changed += OnChange;
    }

    public void OnChange()
    {
        _loggerImplementation = _loggerFactory.CreateLogger<T>();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _loggerImplementation.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _loggerImplementation.IsEnabled(logLevel);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _loggerImplementation.BeginScope(state);
    }
}