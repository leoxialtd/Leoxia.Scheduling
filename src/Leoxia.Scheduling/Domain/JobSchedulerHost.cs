using Microsoft.Extensions.Hosting;

namespace Leoxia.Scheduling.Domain;

/// <summary>
/// Adapter for <see cref="IHostedService"/>
/// </summary>
internal class SchedulingService : IHostedService
{
    private readonly JobTimer _timer;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public SchedulingService(
        JobTimer timer,
        IHostApplicationLifetime applicationLifetime)
    {
        _timer = timer;
        _applicationLifetime = applicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _applicationLifetime.ApplicationStarted.Register(OnStart);
        return Task.CompletedTask;
    }

    private void OnStart()
    {
        _timer.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _timer.Stop();
    }
}