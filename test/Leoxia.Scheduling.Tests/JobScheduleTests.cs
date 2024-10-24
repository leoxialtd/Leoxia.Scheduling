using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Domain;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class JobScheduleTests : ScheduleTestBase
    {
        [Test]
        public void Scheduled_job_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Build();
            Timer.Tick(); // Will initialize the run

            // AND two ticks is raised
            Time.Set(now + TimeSpan.FromMilliseconds(1001));
            Timer.Tick();

            // THEN job should be invoked twice
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromSeconds(1));
            Assert.That(invoked, Is.True);
            Assert.That(job.Counter, Is.EqualTo(1));
        }

        [Test]
        public void Scheduled_job_should_be_scheduled_several_times()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);

            var counter = 0;
            var mre = new ManualResetEvent(false);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EveryMinutes()
                .ThenRun(j =>
                {
                    var c = Interlocked.Increment(ref counter);
                    if (c == 2)
                    {
                        mre.Set();
                    }
                })
                .Build();
            Timer.Tick(); // Will initialize the run

            // AND two ticks is raised
            IncreaseTime(TimeSpan.FromSeconds(61));
            Timer.Tick();
            IncreaseTime(TimeSpan.FromSeconds(61));
            Timer.Tick();

            // THEN job should be invoked twice
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromSeconds(2));
            Assert.That(invoked, Is.True);

            mre.WaitOne(TimeSpan.FromSeconds(2));
            Assert.That(job.Counter, Is.EqualTo(2));
        }

        [Test]
        public async Task Scheduled_job_should_not_be_scheduled_timer_stops()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Build();

            // AND timer is stopped
            var jobTimer = Resolve<JobTimer>();
            await jobTimer.Stop();

            // AND two ticks is raised
            Time.Set(now + TimeSpan.FromMilliseconds(1001));
            Timer.Tick();
            Time.Set(now + TimeSpan.FromMilliseconds(1001));
            Timer.Tick();

            // THEN job should not be invoked
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromMilliseconds(50));
            Assert.That(invoked, Is.False);
            Assert.That(job.Counter, Is.EqualTo(0));
        }

        [Test]
        public void Scheduled_cron_job_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = ParseTime("2024/10/24 04:00:01");
            Time.Set(now);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .Cron("0 0 3 * * *")
                .Build();

            // AND several ticks are raised
            now = ParseTime("2024/10/25 03:00:00");
            Time.Set(now);
            Timer.Tick();

            // THEN job should be invoked once
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromMilliseconds(2000));
            Assert.That(invoked, Is.True);
            Assert.That(job.Counter, Is.EqualTo(1));
        }

        [Test]
        public void Scheduled_job_should_be_scheduled_once()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Once()
                .Build();

            // AND several ticks are raised
            now += TimeSpan.FromMilliseconds(1001);
            Time.Set(now);
            Timer.Tick();

            now += TimeSpan.FromMilliseconds(1001);
            Time.Set(now);
            Timer.Tick();

            // THEN job should be invoked once
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromMilliseconds(50));
            Assert.That(invoked, Is.True);
            Assert.That(job.Counter, Is.EqualTo(1));
        }
    }
}