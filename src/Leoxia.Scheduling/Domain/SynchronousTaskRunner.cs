using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

public class SynchronousTaskRunner : ITaskRunner
{
    public async Task Run(Func<Task> action)
    {
        await action();
    }
}