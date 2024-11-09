
using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class Job(Type type, IInvocableResolver resolver) : IJob
{
    public DateTimeOffset CreationDate { get; set; }

    public string Name { get; set; } = type.Name;

    public Type Type { get; } = type;

    public object[] Parameters { get; set; } = [];

    public IRunScheduler RunScheduler { get; set; } = new DummyRunScheduler();

    public int? MaxRuns { get; set; }

    public IInvocableResolver Resolver { get; set; } = resolver;

    public Queue<Func<IInvocable,Task>> ExecutionQueue { get; } = new Queue<Func<IInvocable, Task>>();

    public bool Overlapping { get; set; } = true;
}

internal class DummyRunScheduler : IRunScheduler
{
    public DateTimeOffset? GetNextRun(DateTimeOffset now)
    {
        return null;
    }
}

internal interface IRunScheduler
{
    DateTimeOffset? GetNextRun(DateTimeOffset now);
}