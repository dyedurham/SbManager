using System;
using System.Linq;
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

        public Queue Build(QueueCriteria criteria)
        {
            return _busMonitor.GetOverview(criteria.RequireFresh).Queues.Single(q => String.Equals(q.Name, criteria.Queue, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
