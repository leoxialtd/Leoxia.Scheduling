using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class DummyInvocable : IInvocable
{
    public static readonly IInvocable Instance = new DummyInvocable();

    private DummyInvocable()
    {
    }

    public Task Invoke()
    {
        return Task.CompletedTask;
    }
}