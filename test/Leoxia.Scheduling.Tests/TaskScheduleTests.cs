using Leoxia.Scheduling.Abstractions;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    public class TaskScheduleTests : ScheduleTestBase
    {
        [Test]
        public async Task Action_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick(); // Will initialize the time

            var mre = new ManualResetEvent(false);

            // WHEN a job is scheduled
            var job = scheduler.Schedule(() =>
                {
                    mre.Set();
                })
                .EverySeconds()
                .Build();
            Timer.Tick(); // Will initialize the run

            // AND two ticks is raised
            Time.Set(now + TimeSpan.FromMilliseconds(1001));
            Timer.Tick();

            // THEN job should be invoked twice
            var invoked = mre!.WaitOne(TimeSpan.FromSeconds(1));
            Assert.That(invoked, Is.True);

            // WHEN triggering job
            mre.Reset();
            await scheduler.Trigger(job);

            // THEN it should be triggered (not depending on timer)
            invoked = mre!.WaitOne(TimeSpan.FromSeconds(1));
            Assert.That(invoked, Is.True);
        }

        [Test]
        public async Task Task_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick(); // Will initialize the time

            var mre = new ManualResetEvent(false);

            Task SetMre()
            {
                mre.Set();
                return Task.CompletedTask;
            }

            // WHEN a job is scheduled
            var job = scheduler.Schedule(SetMre)
                .EverySeconds()
                .Build();
            Timer.Tick(); // Will initialize the run

            // AND two ticks is raised
            Time.Set(now + TimeSpan.FromMilliseconds(1001));
            Timer.Tick();

            // THEN job should be invoked twice
            var invoked = mre!.WaitOne(TimeSpan.FromSeconds(1));
            Assert.That(invoked, Is.True);

            // WHEN triggering job
            mre.Reset();
            await scheduler.Trigger(job);

            // THEN it should be triggered (not depending on timer)
            invoked = mre!.WaitOne(TimeSpan.FromSeconds(1));
            Assert.That(invoked, Is.True);
        }

    }
}
