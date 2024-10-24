using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling.Domain;

internal class ScopedInvocable : IScopedInvocable
{
    private readonly IServiceScope _scope;

    public ScopedInvocable(IJob job, IServiceScopeFactory scopeFactory)
    {
        _scope = scopeFactory.CreateScope();
        object instance;
        if (job.Parameters.Length > 0)
        {
            instance = ActivatorUtilities.CreateInstance(_scope.ServiceProvider, job.Type, job.Parameters);
        }
        else
        {
            instance = _scope.ServiceProvider.GetRequiredService(job.Type);
        }

        if (instance is IInvocable invocable)
        {
            Invocable = invocable;
        }
        else
        {
            Invocable = DummyInvocable.Instance;
        }
    }

    public IInvocable Invocable { get; }

    public void Dispose()
    {
        _scope.Dispose();
    }
}