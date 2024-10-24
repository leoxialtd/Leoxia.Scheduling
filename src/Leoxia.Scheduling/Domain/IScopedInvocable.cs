using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal interface IScopedInvocable : IDisposable
{
    IInvocable Invocable { get; }
}