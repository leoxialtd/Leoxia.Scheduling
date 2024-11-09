using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

public class TaskRunner : ITaskRunner
{
    public Task Run(Func<Task> action)
    {
        return Task.Run(action);
    }
}