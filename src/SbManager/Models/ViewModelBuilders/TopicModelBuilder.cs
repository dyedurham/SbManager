using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class TopicCriteria
    {
        public TopicCriteria(string tid)
        {
            Topic = tid;
        }
        public string Topic { get; set; }
    }
    public class TopicModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Topic, TopicCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public TopicModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public Task<Topic> Build(TopicCriteria criteria)
        {
            return _busMonitor.GetTopic(criteria.Topic);
        }
    }
}
