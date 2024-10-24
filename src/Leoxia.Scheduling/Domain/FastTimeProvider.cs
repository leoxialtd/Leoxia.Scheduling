using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

public class StandardTimeProvider : ITimeProvider
{
    public DateTimeOffset UtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}


public class FastTimeProvider : IFastTimeProvider
{
    private DateTimeOffset _utcNow;

    public FastTimeProvider()
    {
        _utcNow = DateTimeOffset.UtcNow;
    }

    public void Set(DateTimeOffset now)
    {
        _utcNow = now;
    }

    public DateTimeOffset UtcNow()
    {
        return _utcNow;
    }
}