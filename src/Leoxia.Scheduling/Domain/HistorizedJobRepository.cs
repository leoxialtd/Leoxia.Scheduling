using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain
{
    internal class HistorizedJobRepository : IHistorizedJobRepository
    {
        private readonly ConcurrentBag<IHistorizedJobRun> _runs = new ConcurrentBag<IHistorizedJobRun>();

        public HistorizedJobRepository()
        {
        }

        public void Add(IHistorizedJobRun historizedRun)
        {
            _runs.Add(historizedRun);
        }

        public IEnumerable<IHistorizedJobRun> GetJobRuns()
        {
            return _runs.ToArray();
        }
    }
}
