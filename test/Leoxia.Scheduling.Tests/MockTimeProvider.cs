using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Tests;

public class MockTimeProvider : ITimeProvider
{
    private DateTimeOffset _now;

    public MockTimeProvider()
    {
        _now = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset UtcNow()
    {
        return _now;
    }

    public void Set(DateTimeOffset now)
    {
        _now = now;
    }
}