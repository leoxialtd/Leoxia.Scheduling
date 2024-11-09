namespace Leoxia.Scheduling.Abstractions;

public interface ITaskRunner
{
    Task Run(Func<Task> action);
}