namespace Leoxia.Scheduling.Abstractions;

public interface IJobBuilder
{
    IJobBuilder WithName(string name);

    IJobBuilder WithParameters(params object[] parameters);

    IJobBuilder Cron(string cronExpression);

    IJobBuilder Every(TimeSpan timeSpan);

    IJobBuilder Times(int times);

    IJobBuilder ResolveWith(Func<IJob, IInvocable> invocableFactory);

    IJobBuilder ThenRun(Action<IInvocable> action);

    IJob Build();
}