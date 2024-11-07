using Leoxia.Scheduling.Abstractions;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    public class JobResolverTests : ScheduleTestBase
    {
        [Test]
        public void Resolve_should_be_done_with_custom_resolver()
        {
            var scheduler = BuildScheduler();

            var now = DateTimeOffset.UtcNow;
            Time.Set(now);
            Timer.Tick();

            ManualResetEvent mre = new ManualResetEvent(false);

            MyCustomJob? capturedJob = null;
            scheduler.Schedule<MyCustomJob>()
                .ResolveWith(j =>
                {
                    return new MyCustomJob("Custom" + j.Type.Name);
                })
                .Every(TimeSpan.FromSeconds(1))
                .ThenRun(j =>
                {
                    capturedJob = (MyCustomJob)j;
                    mre.Set();
                })
                .Build();
            
            IncreaseTime(TimeSpan.FromSeconds(1));
            Timer.Tick();

            var captured = mre.WaitOne(TimeSpan.FromSeconds(1));
            Assert.That(captured, Is.True);
            Assert.That(capturedJob, Is.Not.Null);
            Assert.That(capturedJob!.Name, Is.Not.Null);
            Assert.That(capturedJob!.Name, Is.EqualTo("CustomMyCustomJob"));
        }
    }

    public class MyCustomJob : MyJob, IInvocable
    {
        public string Name { get; private set; }

        public MyCustomJob(string name)
        {
            Name = name;
        }
    }
}
