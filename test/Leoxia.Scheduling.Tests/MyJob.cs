using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Tests;

public class MyJob : IInvocable
{
    private readonly ManualResetEvent _mre = new (false);
    private int _counter;

    public int Counter => _counter;

    public bool Invoked { get; private set; }

    public Task Invoke()
    {
        Invoked = true;
        Interlocked.Increment(ref _counter);
        _mre.Set();
        return Task.CompletedTask;
    }

    public bool WaitForInvocation(TimeSpan timeSpan)
    {
        return _mre.WaitOne(timeSpan);
    }
}