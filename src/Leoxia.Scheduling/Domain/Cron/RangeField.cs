namespace Leoxia.Scheduling.Domain.Cron;

public class RangeField : CronField
{
    private readonly int _start;
    private readonly int _end;

    public RangeField(int start, int end, int min, int max) : base(min, max)
    {
        _start = start;
        _end = end;
    }

    public override bool Matches(int value) => value >= _start && value <= _end;
    public override int IncrementToReachNext(int currentValue, int? max = null)
    {
        var increment = 0;
        if (Min == 0)
        {
            increment = 1;
        }

        if (currentValue < _start)
        {
            return _start - currentValue;
        }
        
        if (currentValue >= _end)
        {
            return GetMax(max) - currentValue + _start + increment;
        }

        return 1;
    }

    public override bool IsFirstMatch(int value)
    {
        return value == _start;
    }

    public override string ToString()
    {
        return $"{_start}-{_end}";
    }
}