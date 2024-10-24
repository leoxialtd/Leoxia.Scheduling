using Leoxia.Scheduling.Abstractions;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class SuccessorScheduleTests : ScheduleTestBase
{
    [Test]
    public void Successor_task_should_be_scheduled()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);

        var mre = new ManualResetEvent(false);

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>()
            .EverySeconds()
            .ThenRun(_ => { mre.Set(); })
            .Build();

        // AND a tick is raised
        Time.Set(now + TimeSpan.FromMilliseconds(1001));
        Timer.Tick();

        // THEN task should be invoked
        var invoked = mre.WaitOne(TimeSpan.FromSeconds(2));
        Assert.That(invoked, Is.True);
    }

    [Test]
    public void Successor_task_should_be_scheduled_once()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var counter = 0;

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);
        var mre = new ManualResetEvent(false);

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>()
            .EverySeconds()
            .Once()
            .ThenRun(_ => {
                Interlocked.Increment(ref counter);
                mre.Set();
            })
            .Build();

        // AND a tick is raised
        IncreaseTime(TimeSpan.FromMilliseconds(1001));
        Timer.Tick();

        IncreaseTime(TimeSpan.FromMilliseconds(1001));
        Timer.Tick();

        // THEN task should be invoked
        var invoked = mre.WaitOne(TimeSpan.FromSeconds(2));
        Assert.That(invoked, Is.True);
        Assert.That(counter, Is.EqualTo(1));
    }
}