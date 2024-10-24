using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain
{
    internal class TimerFactory : ITimerFactory
    {
        public ITimer Create(Action onTimerTick)
        {
            void OnTimerTick(object? state)
            {
                onTimerTick();
            }

            return new Timer(OnTimerTick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }
}
