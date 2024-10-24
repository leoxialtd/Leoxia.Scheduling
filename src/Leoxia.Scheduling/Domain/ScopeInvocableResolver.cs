using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling.Domain;

internal class ScopeInvocableResolver : IInvocableResolver
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopeInvocableResolver(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IScopedInvocable Resolve(IJob job)
    {
        return new ScopedInvocable(job, _serviceScopeFactory);
    }
}