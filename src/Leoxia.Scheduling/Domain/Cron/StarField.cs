namespace Leoxia.Scheduling.Domain.Cron;

public class StarField : CronField
{
    public StarField(int min, int max) : base(min, max)
    {
    }

    public override bool Matches(int value) => true;
    public override int IncrementToReachNext(int currentValue, int? max = null) => 1;
    public override bool IsFirstMatch(int value)
    {
        return value == Min;
    }

    public override string ToString()
    {
        return "*";
    }
}