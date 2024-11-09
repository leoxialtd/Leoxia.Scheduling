using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    public class MaxTimeTests : ScheduleTestBase
    {
        public MaxTimeTests() : base(ConfigureServices)
        {
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MyOverlappingJob>();
        }

        [Test]
        public void Job_should_not_be_invoked_more_than_max_times()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick(); // Will initialize the time

            // WHEN a job is scheduled
            scheduler.Schedule<MyOverlappingJob>()
                .EverySeconds()
                .Times(3)
                .Build();

            // AND five ticks is raised
            for (int i = 0; i < 5; i++)
            {
                IncreaseTime(TimeSpan.FromSeconds(1));
                Timer.Tick();
            }

            // THEN job should be invoked twice
            var job = Resolve<MyOverlappingJob>();
            var invoked = job.WaitForInvocationNumber(5, TimeSpan.FromSeconds(2));
            Assert.That(invoked, Is.False);
            Assert.That(job.InvocationNumber, Is.EqualTo(3));
        }
    }
}
