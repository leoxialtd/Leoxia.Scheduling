using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling.Domain;

internal class JobSchedule : IJobScheduler
{
    private readonly JobRepository _repository;
    private readonly ScopeInvocableResolver _resolver;

    public JobSchedule(
        JobRepository repository,
        IServiceScopeFactory serviceScopeFactory)
    {
        _repository = repository;
        _resolver = new ScopeInvocableResolver(serviceScopeFactory);
    }


    public IJobBuilder Schedule(Type invocableType)
    {
        return new JobBuilder(_repository, invocableType, _resolver);
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