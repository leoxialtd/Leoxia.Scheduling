using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Tests;

public class MockTimerFactory : ITimerFactory
{
    private Action? _onTimerTick;

    public ITimer Create(Action onTimerTick)
    {
        _onTimerTick = onTimerTick;
        return new MockTimer(_onTimerTick);
    }

    public void Tick()
    {
        _onTimerTick?.Invoke();
    }
}