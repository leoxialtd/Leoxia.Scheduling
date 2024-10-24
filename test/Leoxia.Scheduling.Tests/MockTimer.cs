namespace Leoxia.Scheduling.Tests;

internal sealed class MockTimer : ITimer
{
    private readonly Action _onTimerTick;

    public MockTimer(Action onTimerTick)
    {
        _onTimerTick = onTimerTick;
    }

    public void Dispose()
    {
        // Do Nothing
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public bool Change(TimeSpan dueTime, TimeSpan period)
    {
        if (dueTime == TimeSpan.Zero)
        {
            _onTimerTick();
        }

        return true;
    }
}