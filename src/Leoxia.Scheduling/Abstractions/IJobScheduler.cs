namespace Leoxia.Scheduling.Abstractions
{
    public interface IJob
    {
        string Name { get; }

        Type Type { get; }

        object[] Parameters { get; }
    }

    public interface IJobScheduler
    {
        IJobBuilder Schedule(Type invocableType);

        Task Trigger(IJob job);
    }
}