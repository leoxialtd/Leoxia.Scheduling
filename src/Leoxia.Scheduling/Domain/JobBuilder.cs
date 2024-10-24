using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class JobBuilder : IJobBuilder
{
    private readonly JobRepository _repository;
    private readonly Job _job;

    public JobBuilder(JobRepository repository, Type invocableType, IInvocableResolver resolver)
    {
        _repository = repository;
        _job = new Job(invocableType, resolver);
    }

    public IJobBuilder WithName(string name)
    {
        _job.Name = name;
        return this;
    }

    public IJobBuilder WithParameters(params object[] parameters)
    {
        _job.Parameters = parameters;
        return this;
    }

    public IJobBuilder Cron(string cronExpression)
    {
        _job.RunScheduler = new CronScheduler(cronExpression);
        return this;
    }

    public IJobBuilder Every(TimeSpan timeSpan)
    {
        _job.RunScheduler = new TimeSpanScheduler(timeSpan);
        return this;
    }

    public IJobBuilder Times(int times)
    {
        _job.MaxRuns = times;
        return this;
    }

    public IJobBuilder ResolveWith(Func<IJob, IInvocable> invocableFactory)
    {
        _job.Resolver = new ActionResolver(invocableFactory);
        return this;
    }

    public IJobBuilder ThenRun(Action<IInvocable> action)
    {
        _job.ExecutionQueue.Enqueue(action);
        return this;
    }

    public IJob Build()
    {
        _repository.Add(_job);
        return _job;
    }
}