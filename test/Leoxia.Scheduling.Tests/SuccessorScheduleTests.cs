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
    public void Several_successor_task_should_be_scheduled()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);

        var mre = new ManualResetEvent(false);
        var mre2 = new ManualResetEvent(false);

        int index = 0, index1 = 0, index2 = 0;
        
        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>()
            .EverySeconds()
            .ThenRun(_ =>
            {
                index1 = Interlocked.Increment(ref index);
                mre.Set();
                return Task.CompletedTask;
            })
            .ThenRun(_ =>
            {
                index2 = Interlocked.Increment(ref index);
                mre2.Set();
            })
            .Build();

        // AND a tick is raised
        Time.Set(now + TimeSpan.FromMilliseconds(1001));
        Timer.Tick();

        // THEN tasks should be invoked
        var invoked = mre.WaitOne(TimeSpan.FromSeconds(2));
        Assert.That(invoked, Is.True);
        invoked = mre2.WaitOne(TimeSpan.FromSeconds(2));
        Assert.That(invoked, Is.True);

        // AND Sequence should be respected
        Assert.That(index1, Is.EqualTo(1));
        Assert.That(index2, Is.EqualTo(2));
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