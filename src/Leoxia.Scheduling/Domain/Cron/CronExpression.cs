using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Leoxia.Scheduling.Domain.Cron;

public class CronExpression
{
    private static readonly Calendar _calendar = new GregorianCalendar();

    public CronField Seconds { get; private set; }
    public CronField Minutes { get; private set; }
    public CronField Hours { get; private set; }
    public CronField DayOfMonth { get; private set; }
    public CronField Month { get; private set; }
    public CronField DayOfWeek { get; private set; }

    public CronExpression(string cronExpression)
    {
        var parts = cronExpression.Split(' ');
        if (parts.Length != 6)
        {
            throw new ArgumentException("Invalid cron expression format. It should consist of 6 fields.");
        }

        Seconds = ParseField(parts[0], 0, 59, "Seconds");
        Minutes = ParseField(parts[1], 0, 59, "Minutes");
        Hours = ParseField(parts[2], 0, 23, "Hours");
        DayOfMonth = ParseField(parts[3], 1, 31, "DayOfMonth");
        Month = ParseField(parts[4], 1, 12, "Month");
        DayOfWeek = ParseField(parts[5], 1, 7, "DayOfWeek", true, ConvertDayOfWeek);
    }

    private int ConvertDayOfWeek(int arg)
    {
        if (arg == 0)
        {
            return 7;
        }

        return arg;
    }

    private CronField ParseField(string field, int min, int max, string fieldName, 
        bool canCompose = true,
        Func<int, int>? converter = null)
    {
        if (field == "*")
        {
            return new StarField(min, max);
        }

        if (canCompose && field.Contains(","))
        {
            var parts = field.Split(',');
            return new CompositeField(
                parts.Select(x => ParseField(x, min, max, fieldName, false, converter)),
                min, max);
        }

        if (field.Contains("/"))
        {
            var intervalParts = field.Split('/');
            var first = intervalParts[0];
            if (first.Contains("-"))
            {
                var rangeParts = first.Split('-');
                var start = int.Parse(rangeParts[0]);
                var end = int.Parse(rangeParts[1]);
                if (converter != null)
                {
                    start = converter(start);
                    end = converter(end);
                }

                if (start < min || end > max)
                {
                    throw new ArgumentOutOfRangeException(nameof(field), $"cron expression is invalid, field {fieldName} range should be between {min} {max}");
                }

                var interval = int.Parse(intervalParts[1]);
                return new RangeIntervalField(start, end, interval, min, max);
            }
            else
            {
                var start = int.Parse(first == "*" ? "0" : first);
                var interval = int.Parse(intervalParts[1]);
                if (converter != null)
                {
                    start = converter(start);
                    interval = converter(interval);
                }

                return new IntervalField(start, interval, min, max);
            }
        }

        if (field.Contains("-"))
        {
            var rangeParts = field.Split('-');
            var start = int.Parse(rangeParts[0]);
            var end = int.Parse(rangeParts[1]);
            if (converter != null)
            {
                start = converter(start);
                end = converter(end);
            }

            if (start < min || end > max)
            {
                throw new ArgumentOutOfRangeException(nameof(field), $"cron expression is invalid, field {fieldName} range should be between {min} {max}");
            }

            return new RangeField(start, end, min, max);
        }

        var target = int.Parse(field);
        if (converter != null)
        {
            target = converter(target);
        }

        return new SpecificValueField(target, min, max);
    }

    public DateTimeOffset GetNextTrigger(DateTimeOffset now)
    {
        DateTimeOffset next = now.Add(TimeSpan.FromSeconds(1));

        // Remove precision below second
        if (next.Microsecond > 0)
        {
            next = next.AddMicroseconds(-next.Microsecond);
        }

        if (next.Millisecond > 0)
        {
            next = next.AddMilliseconds(-next.Millisecond);
        }

        var ticks = next.Ticks % 1000000;
        if (ticks > 0)
        {
            next = next.AddTicks(-ticks);
        }

        while (true)
        {
            if (!Seconds.Matches(next.Second))
            {
                var seconds = Seconds.IncrementToReachNext(next.Second);
                next = next.AddSeconds(seconds);
                continue;
            }

            if (!Minutes.Matches(next.Minute))
            {
                if (!(Seconds is SpecificValueField))
                {
                    next = next.AddSeconds(-next.Second);
                }

                var minutes = Minutes.IncrementToReachNext(next.Minute);
                next = next.AddMinutes(minutes);
                continue;
            }

            if (!Hours.Matches(next.Hour))
            {
                if (!(Minutes is SpecificValueField))
                {
                    next = next.AddMinutes(-next.Minute);
                }

                var hours = Hours.IncrementToReachNext(next.Hour);
                next = next.AddHours(hours);
                continue;
            }

            if (!DayOfMonth.Matches(next.Day))
            {
                if (!Hours.IsFirstMatch(next.Hour))
                {
                    next = next.AddHours(-next.Hour);
                }

                var max = _calendar.GetDaysInMonth(next.Year, next.Month);
                var dayOfMonth = DayOfMonth.IncrementToReachNext(next.Day, max);
                next = next.AddDays(dayOfMonth);
                continue;
            }

            if (!Month.Matches(next.Month))
            {
                // Remove component days when the day of month is not specific
                // As we change month the first occurence may be earlier
                if (!DayOfMonth.IsFirstMatch(next.Day))
                {
                    next = next.AddDays(1 - next.Day);
                }

                var month = Month.IncrementToReachNext(next.Month);
                next = next.AddMonths(month);
                continue;
            }

            var dayOfWeek = next.DayOfWeek == 0 ? 7 : (int)next.DayOfWeek;
            if (!DayOfWeek.Matches(dayOfWeek))
            {
                var daysToAdd = DayOfWeek.IncrementToReachNext(dayOfWeek);
                next = next.AddDays(daysToAdd);
                continue;
            }

            if (next.Year >= 3000)
            {
                throw new InvalidOperationException("Cron match failed to match an existing next date");
            }

            return next;
        }
    }

    public override string ToString()
    {
        return $"{Seconds} {Minutes} {Hours} {DayOfMonth} {Month} {DayOfWeek}";
    }
}