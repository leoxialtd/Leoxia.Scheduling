namespace Leoxia.Scheduling.Domain.Cron;

internal class CompositeField : CronField
{
    private readonly CronField[] _fields;
    private CronField _field;

    public CompositeField(IEnumerable<CronField> fields, int min, int max) : base(min, max)
    {
        _fields = fields.ToArray();
        _field = fields.First();
    }

    public override bool Matches(int value)
    {
        foreach (var field in _fields)
        {
            if (field.Matches(value))
            {
                _field = field;
                return true;
            }
        }

        return false;
    }

    public override int IncrementToReachNext(int currentValue, int? max = null)
    {
        return _field.IncrementToReachNext(currentValue, max);
    }

    public override bool IsFirstMatch(int value)
    {
        return _field.IsFirstMatch(value);
    }

    public override string ToString()
    {
        return string.Join(',', _fields.Select(x => x.ToString()));
    }
}