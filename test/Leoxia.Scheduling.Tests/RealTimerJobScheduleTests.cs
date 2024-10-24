
using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Domain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RealTimerScheduleTests : ScheduleTestBase
    {
        public RealTimerScheduleTests() : base(ConfigureServices)
        {
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeProvider, StandardTimeProvider>();
            services.AddSingleton<ITimerFactory, TimerFactory>();
        }

        [Test]
        public void Scheduled_job_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Build();

            // THEN job should be invoked
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromSeconds(2));
            Assert.That(invoked, Is.True);
        }

        [Test]
        public async Task Scheduled_job_should_not_be_scheduled_timer_stops()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Build();

            // AND timer is stopped
            var jobTimer = Resolve<JobTimer>();
            await jobTimer.Stop();

            // THEN job should not be invoked
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromMilliseconds(50));
            Assert.That(invoked, Is.False);
            Assert.That(job.Counter, Is.EqualTo(0));
        }

        [Test]
        public void Scheduled_job_should_be_scheduled_once()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .Once()
                .Build();

            // THEN job should be invoked once
            var job = Resolve<MyJob>();
            var invoked = job!.WaitForInvocation(TimeSpan.FromSeconds(2));
            Assert.That(invoked, Is.True);
            Assert.That(job.Counter, Is.EqualTo(1));
        }
    }
}