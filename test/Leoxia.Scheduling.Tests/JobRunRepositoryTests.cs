using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Domain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class JobRunHistoryTests : ScheduleTestBase
{
    public JobRunHistoryTests() : base(ConfigureServices)
    {
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ITaskRunner, SynchronousTaskRunner>();
    }

    [Test]
    public void Scheduled_job_should_not_have_run_in_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IHistorizedJobRepository>();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);
        Timer.Tick(); // Will initialize the time

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>().Daily().Build();

        // THEN it should be present in repo
        var jobRun = repository.GetJobRuns().FirstOrDefault();
        Assert.That(jobRun, Is.Null);
    }

    [Test]
    public async Task Scheduled_job_run_should_be_part_of_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IHistorizedJobRepository>();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);
        Timer.Tick(); // Will initialize the time

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>().Daily().Build();

        // AND run once
        IncreaseTime(TimeSpan.FromDays(2));
        Timer.Tick();

        // THEN it should be present in repo
        var jobRun = repository.GetJobRuns().OrderBy(x => x.Start).FirstOrDefault();
        Assert.That(jobRun, Is.Not.Null);
        Assert.That(jobRun!.Job, Is.Not.Null);

        // Note that the IsRunning property is updated asynchronously
        await Task.Delay(1000);
        Assert.That(jobRun!.IsRunning, Is.False);
    }

    [Test]
    public async Task All_job_runs_should_be_part_of_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IHistorizedJobRepository>();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);
        Timer.Tick(); // Will initialize the time

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>().Daily().Build();

        // AND run n times
        for (int i = 0; i < 10; i++)
        {
            IncreaseTime(TimeSpan.FromDays(1));
            Timer.Tick();
        }

        // THEN it should be present in repo
        var jobRuns = repository.GetJobRuns().ToList();
        Assert.That(jobRuns.Count, Is.EqualTo(10));

        // Note that the IsRunning property is updated asynchronously
        await Task.Delay(1000);
        foreach (var run in jobRuns)
        {
            Assert.That(run.IsRunning, Is.False);
        }
    }

    [Test]
    public void Scheduled_jobs_should_be_part_of_repository()
    {
        // GIVEN a Scheduler and Job repo
        var scheduler = BuildScheduler();
        var repository = Resolve<IHistorizedJobRepository>();

        var now = DateTimeOffset.UtcNow;
        Time.Set(now);
        Timer.Tick(); // Will initialize the time

        // WHEN a job is scheduled
        scheduler.Schedule<MyJob>().WithName("MyMonthlyJob").Monthly().Build();
        scheduler.Schedule<MyJob>().WithName("MyDailyJob").Daily().Build();
        scheduler.Schedule<MyJob>().WithName("MyHourlyJob").Hourly().Build();
        scheduler.Schedule<MyJob>().WithName("MyMinutesJob").Hourly().Build();

        // AND run once
        IncreaseTime(TimeSpan.FromDays(32));
        Timer.Tick();

        // THEN it should be present in repo
        var monthlyJob = repository.GetJobRuns().FirstOrDefault(x => x.Job.Name == "MyMonthlyJob");
        Assert.That(monthlyJob, Is.Not.Null);

        var dailyJob = repository.GetJobRuns().FirstOrDefault(x => x.Job.Name == "MyDailyJob");
        Assert.That(dailyJob, Is.Not.Null);
        
        var hourlyJob = repository.GetJobRuns().FirstOrDefault(x => x.Job.Name == "MyHourlyJob");
        Assert.That(hourlyJob, Is.Not.Null);
        
        var minutesJob = repository.GetJobRuns().FirstOrDefault(x => x.Job.Name == "MyMinutesJob");
        Assert.That(minutesJob, Is.Not.Null);
    }

}