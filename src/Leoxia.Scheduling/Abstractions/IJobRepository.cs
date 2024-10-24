namespace Leoxia.Scheduling.Abstractions
{
    public interface IJobRepository
    {
        IEnumerable<IJob> GetJobs();
    }
}