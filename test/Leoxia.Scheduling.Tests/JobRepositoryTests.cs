using Leoxia.Scheduling.Abstractions;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class JobRepositoryTests : ScheduleTestBase
{

    [Test]
    public void Scheduled_job_should_be_part_of_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IJobRepository>();

        // WHEN a job is scheduled
        var jobBuilt = scheduler.Schedule<MyJob>().Daily().Build();

        // THEN it should be present in repo
        var job = repository.GetJobs().FirstOrDefault();
        Assert.That(job, Is.Not.Null);
        Assert.That(job!.Name, Is.EqualTo("MyJob"));
        Assert.That(jobBuilt.Name, Is.EqualTo("MyJob"));
        Assert.That(jobBuilt.Type.Name, Is.EqualTo("MyJob"));
        Assert.That(jobBuilt.Parameters, Is.Empty);

        var invocable = Resolve<MyJob>();
        Assert.That(invocable.Invoked, Is.False);
    }

    [Test]
    public void Scheduled_jobs_should_be_part_of_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IJobRepository>();

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>().WithName("MyMonthlyJob").Monthly().Build();
        scheduler.Schedule<MyJob>().WithName("MyDailyJob").Daily().Build();
        scheduler.Schedule<MyJob>().WithName("MyHourlyJob").Hourly().Build();
        scheduler.Schedule<MyJob>().WithName("MyMinutesJob").Hourly().Build();

        // THEN it should be present in repo
        var monthlyJob = repository.GetJobs().FirstOrDefault(x => x.Name == "MyMonthlyJob");
        Assert.That(monthlyJob, Is.Not.Null);

        var dailyJob = repository.GetJobs().FirstOrDefault(x => x.Name == "MyDailyJob");
        Assert.That(dailyJob, Is.Not.Null);
        
        var hourlyJob = repository.GetJobs().FirstOrDefault(x => x.Name == "MyHourlyJob");
        Assert.That(hourlyJob, Is.Not.Null);
        
        var minutesJob = repository.GetJobs().FirstOrDefault(x => x.Name == "MyMinutesJob");
        Assert.That(minutesJob, Is.Not.Null);
    }

}