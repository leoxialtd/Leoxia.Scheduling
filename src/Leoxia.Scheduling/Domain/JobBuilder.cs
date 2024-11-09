using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class JobBuilder : IJobBuilder
{
    private readonly JobRepository _repository;
    private readonly IFastTimeProvider _provider;
    private readonly Job _job;

    public JobBuilder(
        JobRepository repository,
        Type invocableType,
        IInvocableResolver resolver,
        IFastTimeProvider provider)
    {
        _repository = repository;
        _provider = provider;
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

    public IJobBuilder ThenRun(Func<IInvocable, Task> action)
    {
        _job.ExecutionQueue.Enqueue(action);
        return this;
    }

    public IJobBuilder PreventOverlap()
    {
        _job.Overlapping = false;
        return this;
    }

    public IJob Build()
    {
        _job.CreationDate = _provider.UtcNow();
        _repository.Add(_job);
        return _job;
    }
}