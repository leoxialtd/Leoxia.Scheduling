using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling.Domain;

internal class JobSchedule : IJobScheduler
{
    private readonly JobRepository _repository;
    private readonly IFastTimeProvider _timeProvider;
    private readonly ScopeInvocableResolver _resolver;

    public JobSchedule(
        JobRepository repository,
        IServiceScopeFactory serviceScopeFactory,
        IFastTimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
        _resolver = new ScopeInvocableResolver(serviceScopeFactory);
    }


    public IJobBuilder Schedule(Type invocableType)
    {
        return new JobBuilder(_repository, invocableType, _resolver, _timeProvider);
    }

    public IJobBuilder Schedule(IInvocable invocable)
    {
        return new JobBuilder(_repository, typeof(IInvocable), new InstanceResolver(invocable), _timeProvider);
    }


    public Task Trigger(IJob job)
    {
        var concreteJob = (Job)job;
        using (var jobInvoker = concreteJob.Resolver.Resolve(job))
        {
            return jobInvoker.Invocable.Invoke();
        }
    }
}

internal class InstanceResolver : IInvocableResolver
{
    private readonly IScopedInvocable _invocable;

    public InstanceResolver(IInvocable invocable)
    {
        _invocable = new InstanceInvocable(invocable);
    }

    public IScopedInvocable Resolve(IJob job)
    {
        return _invocable;
    }
}

internal sealed class InstanceInvocable : IScopedInvocable
{
    public InstanceInvocable(IInvocable invocable)
    {
        Invocable = invocable;
    }

    public void Dispose()
    {
        // Do nothing
    }

    public IInvocable Invocable { get; }
}