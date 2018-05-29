using System;
using System.Linq;
using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class QueueCriteria
    {
        public QueueCriteria(string qid, bool requireFresh = false)
        {
            Queue = qid;
            RequireFresh = requireFresh;
        }
        public string Queue { get; set; }
        public bool RequireFresh { get; set; }
    }

    public class QueueModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Queue, QueueCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public QueueModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public async Task<Queue> Build(QueueCriteria criteria)
        {
            return (await _busMonitor.GetOverview(criteria.RequireFresh)).Queues.Single(q => String.Equals(q.Name, criteria.Queue, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
