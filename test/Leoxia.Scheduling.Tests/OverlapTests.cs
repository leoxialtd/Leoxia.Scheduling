using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    public class OverlapTests : ScheduleTestBase
    {
        public OverlapTests() : base(ConfigureServices)
        {
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MyOverlappingJob>();
        }

        [Test]
        public void Job_could_overlap()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick(); // Will initialize the time

            // WHEN a job is scheduled
            scheduler.Schedule<MyOverlappingJob>()
                .EverySeconds()
                .Build();

            // AND two ticks is raised
            IncreaseTime(TimeSpan.FromSeconds(1));
            Timer.Tick();
            IncreaseTime(TimeSpan.FromSeconds(1));
            Timer.Tick();

            // THEN job should be invoked twice
            var job = Resolve<MyOverlappingJob>();
            var invoked = job.WaitForInvocationNumber(2, TimeSpan.FromSeconds(5));
            Assert.That(invoked, Is.True);
            Assert.That(job.InvocationNumber, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Job_could_not_overlap_with_prevent_overlap()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick(); // Will initialize the time

            // WHEN a job is scheduled
            scheduler.Schedule<MyOverlappingJob>()
                .EverySeconds()
                .PreventOverlap()
                .Build();

            // AND two ticks is raised
            IncreaseTime(TimeSpan.FromSeconds(1));
            Timer.Tick();
            IncreaseTime(TimeSpan.FromSeconds(1));
            Timer.Tick();

            // THEN job should be invoked twice
            var job = Resolve<MyOverlappingJob>();
            var invoked = job.WaitForInvocationNumber(2, TimeSpan.FromSeconds(5));
            Assert.That(invoked, Is.False);
            Assert.That(job.InvocationNumber, Is.EqualTo(1));
        }
    }

    public class MyOverlappingJob : IInvocable
    {
        private readonly object sync = new object();
        private int? _targetInvocation;
        private readonly ManualResetEvent _mre = new ManualResetEvent(false);
        private int _invocationNumber;

        public int InvocationNumber
        {
            get
            {
                lock (sync)
                {
                    return _invocationNumber;
                }
            }
            private set => _invocationNumber = value;
        }

        public Task Invoke()
        {
            lock (sync)
            {
                InvocationNumber++;
                if (_targetInvocation != null && InvocationNumber == _targetInvocation.Value)
                {
                    _mre.Set();
                }
            }

            return Task.CompletedTask;
        }

        public bool WaitForInvocationNumber(int number, TimeSpan waitTime)
        {
            lock (sync)
            {
                if (InvocationNumber >= number)
                {
                    return true;
                }

                _targetInvocation = number;
            }

            return _mre.WaitOne(waitTime);
        }
    }
}
