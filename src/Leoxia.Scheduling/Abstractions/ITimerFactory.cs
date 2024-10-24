namespace Leoxia.Scheduling.Abstractions;

public interface ITimerFactory
{
    ITimer Create(Action onTimerTick);
}