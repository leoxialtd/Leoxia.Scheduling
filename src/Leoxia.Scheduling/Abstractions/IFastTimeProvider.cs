namespace Leoxia.Scheduling.Abstractions;

public interface IFastTimeProvider : ITimeProvider;

public interface ITimeProvider
{
    DateTimeOffset UtcNow();
}