using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Leoxia.Scheduling.Logging;

public sealed class ConfigurableLoggerFactory : ILoggerFactory
{
    private ILoggerFactory _loggerFactory;

    public ConfigurableLoggerFactory()
    {
        _loggerFactory = new NullLoggerFactory();
    }

    public event Action Changed = delegate { };

    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        Changed();
    }

    public void Dispose()
    {
        // do nothing
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggerFactory.CreateLogger(categoryName);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        _loggerFactory.AddProvider(provider);
    }
}