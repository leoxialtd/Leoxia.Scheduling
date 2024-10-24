
using Leoxia.Scheduling.Abstractions;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class JobTriggerTests : ScheduleTestBase
    {
        [Test]
        public void Scheduled_job_should_be_triggered()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);

            // WHEN a job is scheduled
            var job = scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Build();

            // AND no ticks is raised
            // AND job is triggered manually
            scheduler.Trigger(job);

            // THEN job should be invoked once
            var invocableJob = Resolve<MyJob>();
            var invoked = invocableJob!.WaitForInvocation(TimeSpan.FromMilliseconds(50));
            Assert.That(invoked, Is.True);
            Assert.That(invocableJob.Counter, Is.EqualTo(1));
        }
    }
}