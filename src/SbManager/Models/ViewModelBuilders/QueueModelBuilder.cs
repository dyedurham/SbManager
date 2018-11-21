using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class QueueCriteria
    {
        public QueueCriteria(string qid)
        {
            Queue = qid;
        }
        public string Queue { get; set; }
    }

    public class QueueModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Queue, QueueCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public QueueModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public Task<Queue> Build(QueueCriteria criteria)
        {
            return _busMonitor.GetQueue(criteria.Queue);
        }
    }
}
