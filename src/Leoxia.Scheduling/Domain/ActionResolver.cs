using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class ActionResolver : IInvocableResolver
{
    private readonly Func<IJob, IInvocable> _invocableFactory;

    public ActionResolver(Func<IJob, IInvocable> invocableFactory)
    {
        _invocableFactory = invocableFactory;
    }

    public IScopedInvocable Resolve(IJob job)
    {
        return new DisposableWrapper(_invocableFactory(job));
    }
}

internal sealed class DisposableWrapper(IInvocable invocable) : IScopedInvocable
{
    public IInvocable Invocable { get; } = invocable;

    public void Dispose()
    {
        // Nothing to do here
    }
}