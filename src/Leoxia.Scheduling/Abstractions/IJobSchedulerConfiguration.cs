using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Abstractions;

public interface IJobSchedulerConfiguration
{
    IJobSchedulerConfiguration WithLoggerFactory(ILoggerFactory loggerFactory);


    IJobSchedulerConfiguration OnException(Action<Exception, string> onException);

    IJobSchedulerConfiguration LogExceptions();

    IJobSchedulerConfiguration WithTimerPeriod(TimeSpan period);

    /// <summary>
    /// Start the inner timer for scheduling jobs immediately.
    /// Otherwise, it will be started by the hosted service.
    /// Should be set to true when not hosted in ASP.NET Core application.
    /// </summary>
    /// <returns>Instance to stop the scheduler.</returns>
    Task<IStoppable> Start();

}