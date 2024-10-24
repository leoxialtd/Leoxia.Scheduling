using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class JobCreationTests : ScheduleTestBase
{
    public JobCreationTests() : base(Configure)
    {
    }

    private static void Configure(IServiceCollection obj)
    {
        obj.AddSingleton<MyParameterJob>();
        obj.AddScoped<MyScopedJob>();
        obj.AddTransient<MyTransientJob>();
    }

    [Test]
    public void Scheduled_job_with_parameters_should_be_created()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);

        var mre = new ManualResetEvent(false);

        // WHEN a job is scheduled
        MyParameterJob? job = null;
        scheduler.Schedule<MyParameterJob>()
            .WithParameters(new JobParameter("Hello"))
            .EverySeconds()
            .ThenRun(x =>
            {
                job = (MyParameterJob)x;
                mre.Set();
            })
            .Build();

        // AND one tick is raised
        RaiseNextTick();

        mre.WaitOne(500);
        // THEN job should be invoked once and with the right message
        Assert.That(job, Is.Not.Null);
        Assert.That(job!.Invite, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Scoped_job_should_be_created()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);

        var mre = new ManualResetEvent(false);

        // WHEN a job is scheduled
        MyScopedJob? job = null;
        scheduler.Schedule<MyScopedJob>()
            .EverySeconds()
            .ThenRun(x =>
            {
                job = (MyScopedJob)x;
                mre.Set();
            })
            .Build();

        // AND one tick is raised
        RaiseNextTick();

        mre.WaitOne(500);
        // THEN job should be invoked once and with the right message
        Assert.That(job, Is.Not.Null);
        Assert.That(job!.Invoked, Is.True);
    }


    [Test]
    public void Transient_job_should_be_created()
    {
        // GIVEN Scheduler
        var scheduler = BuildScheduler();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);

        var mre = new ManualResetEvent(false);

        // WHEN a job is scheduled
        MyTransientJob? job = null;
        scheduler.Schedule<MyTransientJob>()
            .EverySeconds()
            .ThenRun(x =>
            {
                job = (MyTransientJob)x;
                mre.Set();
            })
            .Build();

        // AND one tick is raised
        RaiseNextTick();

        mre.WaitOne(500);
        // THEN job should be invoked once and with the right message
        Assert.That(job, Is.Not.Null);
        Assert.That(job!.Invoked, Is.True);
    }
}

#pragma warning disable S2094
internal class MyTransientJob : MyJob;
internal class MyScopedJob: MyJob;
#pragma warning restore S2094
