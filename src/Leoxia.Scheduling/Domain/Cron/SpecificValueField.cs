namespace Leoxia.Scheduling.Domain.Cron;

public class SpecificValueField : CronField
{
    private readonly int _target;
    public SpecificValueField(int target, int min, int max): base(min, max)
    {
        _target = target;
    }

    public override bool Matches(int value)
    {
        return _target == value;
    }

    public override int IncrementToReachNext(int currentValue, int? max = null)
    {
        var increment = 0;
        if (Min == 0)
        {
            increment = 1;
        }

        if (currentValue >= _target)
        {
            // we have to move cycle up
            return GetMax(max) - currentValue + _target + increment;
        }
        
        return _target - currentValue;
    }

    public override bool IsFirstMatch(int value)
    {
        return _target == value;
    }

    public override string ToString()
    {
        return _target.ToString();
    }
}