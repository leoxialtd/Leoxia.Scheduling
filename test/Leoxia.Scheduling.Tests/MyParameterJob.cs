using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Tests;

internal class MyParameterJob : IInvocable
{
    public MyParameterJob(JobParameter parameter)
    {
        Invite = parameter.Value;
    }

    public string Invite { get; private set; }

    public Task Invoke()
    {
        Invite += " World";
        return Task.CompletedTask;
    }
}