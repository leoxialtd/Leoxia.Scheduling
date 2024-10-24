using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal interface IInvocableResolver
{
    IScopedInvocable Resolve(IJob job);
}