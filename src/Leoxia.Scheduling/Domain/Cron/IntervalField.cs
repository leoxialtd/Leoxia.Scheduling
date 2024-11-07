namespace Leoxia.Scheduling.Domain.Cron;

public class IntervalField : CronField
{
    private readonly int _interval;
    private readonly int _start;

    public IntervalField(int start, int interval, int min, int max):base(min, max)
    {
        _start = start;
        _interval = interval;
    }

    public override bool Matches(int value) => (value - _start) % _interval == 0;
    public override int IncrementToReachNext(int currentValue, int? max = null)
    {
        int nextValue;
        if (currentValue < (_start + _interval))
        {
            nextValue = _interval - currentValue + _start;
        }
        else
        {
            nextValue = _interval - (currentValue % _interval) + _start;
        }

        return nextValue;
    }

    public override bool IsFirstMatch(int value)
    {
        return (value - _start) == _interval;
    }

    public override string ToString()
    {
        return $"{_start}/{_interval}";
    }
}