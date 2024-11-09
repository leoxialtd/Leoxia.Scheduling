namespace Leoxia.Scheduling.Abstractions
{
    public interface IJob
    {
        string Name { get; }

        Type Type { get; }

        object[] Parameters { get; }

        DateTimeOffset CreationDate { get; }
    }

    public interface IJobScheduler
    {
        IJobBuilder Schedule(Type invocableType);

        IJobBuilder Schedule(IInvocable invocable);

        Task Trigger(IJob job);
    }
}