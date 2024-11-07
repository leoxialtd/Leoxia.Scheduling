namespace Leoxia.Scheduling.Domain.Cron;

public class RangeIntervalField : CronField
{
    private readonly int _interval;
    private readonly int _start;
    private readonly int _end;

    public RangeIntervalField(int start, int end, int interval, int min, int max):base(min, max)
    {
        _start = start;
        _end = end;
        _interval = interval;
    }

    public override bool Matches(int value)
    {
        return value >= _start && value <= _end && (value % _interval == 0);
    }

    public override int IncrementToReachNext(int currentValue, int? max = null)
    {
        return 1;
    }

    public override bool IsFirstMatch(int value)
    {
        return (value - _start) == _interval;
    }

    public override string ToString()
    {
        return $"{_start}-{_end}/{_interval}";
    }
}