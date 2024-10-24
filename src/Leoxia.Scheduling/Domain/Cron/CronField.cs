namespace Leoxia.Scheduling.Domain.Cron;

public abstract class CronField
{
    public int Min { get; }
    public int Max { get; }

    protected CronField(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public abstract bool Matches(int value);
    public abstract int IncrementToReachNext(int currentValue, int? max = null);
    public abstract bool IsFirstMatch(int value);

    protected int GetMax(int? max)
    {
        if (max != null)
        {
            return max.Value;
        }

        return Max;
    }
}