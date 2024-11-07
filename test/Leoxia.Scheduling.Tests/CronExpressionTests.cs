using Leoxia.Scheduling.Domain.Cron;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    public class CronExpressionTests
    {
        [Theory]
        [TestCase("* * * * * 1", "2024/10/21 00:00:00", "2024/10/21 00:00:01")] // Every second of Monday
        [TestCase("* * * * * 1", "2024/10/22 00:00:00", "2024/10/28 00:00:01")] // Every second of Monday
        [TestCase("* * * * * 7", "2024/10/22 00:00:00", "2024/10/27 00:00:01")] // Every second of Sunday
        [TestCase("0 0 0 1 1 *", "2024/10/22 00:00:00", "2025/01/01 00:00:00")] // Only 1st January at 00:00:00
        [TestCase("0 0 1-3 1 1 *", "2024/10/22 00:00:00", "2025/01/01 01:00:00")] // Only 1st January at 1 - 3 am
        [TestCase("0 0 4-9 1 1 *", "2024/10/22 00:00:00", "2025/01/01 04:00:00")] // Only 1st January at 1 - 3 am
        [TestCase("0 0 1,2,3 1 1 *", "2024/10/22 00:00:00", "2025/01/01 01:00:00")] // Only 1st January at 1 2 and 3 am
        [TestCase("0 0 1,2,3 1 1 *", "2025/01/01 01:01:30", "2025/01/01 02:00:00")] // Only 1st January at 1 2 and 3 am
        [TestCase("*/15 * * * * *", "2025/01/01 01:01:30", "2025/01/01 01:01:45")] // Every 15 seconds
        [TestCase("*/15 * * * * *", "2025/01/01 01:01:45", "2025/01/01 01:02:00")] // Every 15 seconds
        [TestCase("0 * * * * *", "2024/10/18 12:30:30", "2024/10/18 12:31:00")]
        [TestCase("0 0 * * * *", "2024/10/18 12:30:00", "2024/10/18 13:00:00")]
        [TestCase("0 0 12 * * *", "2024/10/18 11:59:59", "2024/10/18 12:00:00")]
        [TestCase("0 0 12 * * 1-5", "2024/10/18 12:00:00", "2024/10/21 12:00:00")] // Next Monday (weekday)
        [TestCase("*/2 0-10/2 12 * * *", "2024/10/18 12:00:01", "2024/10/18 12:00:02")] // Every 2 seconds, every 2 hours between 0-10
        [TestCase("*/2 0-10/2 12 * * *", "2024/10/18 12:08:00", "2024/10/18 12:08:02")] // Every 2 seconds, every 2 hours between 0-10
        [TestCase("*/2 0-10/2 12 * * *", "2024/10/18 12:09:03", "2024/10/18 12:10:00")] // Every 2 seconds, every 2 hours between 0-10
        [TestCase("*/2 0-10/2 12 * * *", "2024/10/18 12:10:59", "2024/10/19 12:00:00")] // Every 2 seconds, every 2 hours between 0-10
        [TestCase("0 0 12 * 5 *", "2024/10/18 11:00:00", "2025/05/01 12:00:00")] // May 1st at 12 PM
        [TestCase("0 30 10 15 * *", "2024/10/14 12:00:00", "2024/10/15 10:30:00")] // 15th of the month
        [TestCase("0 0 12 1 * *", "2024/10/18 12:00:00", "2024/11/01 12:00:00")] // Noon on the 1st of every month
        [TestCase("0 0 0 29 2 *", "2023/02/28 23:59:59", "2024/02/29 00:00:00")] // Leap year check (Feb 29th)
        [TestCase("0 0 12 * * 0", "2024/10/18 12:00:00", "2024/10/20 12:00:00")] // Every Sunday at noon
        [TestCase("0 0 12 * * 7", "2024/10/18 12:00:00", "2024/10/20 12:00:00")] // Same as above, Sunday is treated as both 0 and 7
        [TestCase("0 30 10 31 12 *", "2024/12/01 00:00:00", "2024/12/31 10:30:00")] // December 31st at 10:30 AM
        [TestCase("0 0 0 * 6 *", "2024/05/31 23:59:59", "2024/06/01 00:00:00")] // Midnight on June 1st
        [TestCase("0 0 9 15 * *", "2024/03/15 08:59:59", "2024/03/15 09:00:00")] // 9 AM on the 15th of every month
        [TestCase("*/10 * * * * *", "2024/10/18 12:00:05", "2024/10/18 12:00:10")] // Every 10 seconds
        [TestCase("*/10 0 * * * *", "2024/10/18 12:00:55", "2024/10/18 13:00:00")] // Every 10 seconds at the top of the hour
        [TestCase("0 0 10 1 * 1", "2024/10/18 12:00:00", "2025/09/01 10:00:00")] // 10 AM on the 1st of the month, only on Mondays
        [TestCase("*/2 */10 * * * *", "2024/10/18 10:00:01", "2024/10/18 10:00:02")] // Every 2 seconds, every 10th minute
        [TestCase("0 30 23 * * *", "2024/10/18 22:00:00", "2024/10/18 23:30:00")] // 11:30 PM every day
        [TestCase("*/15 0-10 * * * *", "2024/10/18 09:59:45", "2024/10/18 10:00:00")] // Every 15 seconds within the first 10 minutes of each hour
        [TestCase("0 0 0 * * 1", "2024/10/18 00:00:00", "2024/10/21 00:00:00")] // Midnight on Mondays
        [TestCase("0 0 12 29 2 *", "2023/03/01 00:00:00", "2024/02/29 12:00:00")] // Leap year, February 29th at noon
        [TestCase("*/5 * * * * *", "2024/10/18 12:30:58", "2024/10/18 12:31:00")] // Every 5 seconds, close to the minute boundary
        [TestCase("0 0 0 * 3 5", "2024/03/01 00:00:00", "2024/03/08 00:00:00")] // Midnight on Fridays in March
        [TestCase("0 0 0 * 2 6", "2024/02/29 00:00:00", "2025/02/01 00:00:00")] // Midnight on leap-year February Saturdays
        [TestCase("0 0 0 1 1 *", "2024/12/31 23:59:59", "2025/01/01 00:00:00")] // Midnight on January 1st, any year
        [TestCase("0 0 0 29 2 *", "2023/03/01 00:00:00", "2024/02/29 00:00:00")] // Leap year check (Feb 29th)
         [TestCase("*/30 * 14 29 * 1,5", "2024/12/27 14:00:15", "2025/08/29 14:00:30")] // Every 30 seconds, 2 PM on the 29th of December, only on Mondays and Fridays
        [TestCase("*/2 59 23 31 12 0", "2024/12/31 23:59:59", "2028/12/31 23:59:00")] // Every 2 seconds on Dec 31st, at 11:59 PM, only on Sundays
        [TestCase("*/10 1-10/2 * * * 1,3,5", "2024/10/18 01:00:05", "2024/10/18 01:02:00")] // Every 10 seconds, first 10 minutes of the hour, only Mondays, Wednesdays, and Fridays
        [TestCase("0 0 0 1 1 1-5", "2024/12/31 23:59:59", "2025/01/01 00:00:00")] // Midnight on January 1st, Monday to Friday
        [TestCase("*/5 0-5 0-5 1,15 * *", "2024/10/15 05:04:59", "2024/10/15 05:05:00")] // Every 5 seconds, first 5 minutes and hours, on the 1st and 15th of the month
        [TestCase("0 0 12 29 2 *", "2024/02/28 23:59:59", "2024/02/29 12:00:00")] // Leap year February 29th at noon
        [TestCase("*/1 * * * * *", "2024/10/18 23:59:59", "2024/10/19 00:00:00")] // Every second, tests edge transition between days
        [TestCase("0 0 12 31 12 5", "2024/10/15 12:00:00", "2027/12/31 12:00:00")] // Noon on December 31st, only on Fridays
        public void Cron_should_be_parsed(string cronText, string now, string expected)
        {
            var nowTime = ParseTime(now);
            var cron = new CronExpression(cronText);
            var next = cron.GetNextTrigger(nowTime);
            var expectedTime = ParseTime(expected);
            Assert.That(next, Is.EqualTo(expectedTime));
            if (cronText.StartsWith("*/"))
            {
                cronText = cronText.Replace("*/", "0/");
            }

            if (cronText.EndsWith(" 0"))
            {
                cronText = cronText[..^2] + " 7";
            }

            Assert.That(cron.ToString(), Is.EqualTo(cronText));
        }

        [Test]
        public void Cron_should_discarded_precision_below_seconds()
        {
            var nowTime = ParseTime("2024/10/15 12:00:00");
            var nowWithPrecision = nowTime.AddMicroseconds(1).AddMilliseconds(1).AddTicks(1);
            var expectedNext = nowTime.AddSeconds(1);
            var cron = new CronExpression("* * * * * *");
            var result = cron.GetNextTrigger(nowWithPrecision);
            Assert.That(result, Is.EqualTo(expectedNext));
        }



        private DateTimeOffset ParseTime(string time)
        {
            time += "+00:00";
            var offset = DateTimeOffset.ParseExact(time, "yyyy/MM/dd HH:mm:sszzz", null);
            return offset;
        }

        // Specific case AI generated, probably not correct.
        // [TestCase("*/5 55-59 23 * * 6", "2024/10/18 23:55:01", "2024/10/18 23:55:05")] // Every 5 seconds, last 5 minutes of the day, only on Saturdays
        // [TestCase("5-15/2 10-20/5 * * * 0,7", "2024/12/08 11:05:00", "2024/12/08 11:07:00")] // Every 2 minutes between 5-15, every 5th hour between 10-20, only on Sundays
        // [TestCase("*/7 0-10/3 8-18/2 1-15/5 * 1,2,3", "2024/10/07 09:14:00", "2024/10/08 08:00:00")] // Every 7 seconds, every 3rd minute, between 8AM-6PM, every 5th day of month, Monday to Wednesday
        // [TestCase("*/15 12 * 8 * 2", "2024/08/06 12:14:45", "2024/08/13 12:00:00")] // Every 15 seconds, 12 PM, August, only on Tuesdays
    }
}
