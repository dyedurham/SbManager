using System;
using System.Linq;
using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class TopicCriteria
    {
        public TopicCriteria(string tid, bool requireFresh = false)
        {
            Topic = tid;
            RequireFresh = requireFresh;
        }
        public string Topic { get; set; }
        public bool RequireFresh { get; set; }
    }
    public class TopicModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Topic, TopicCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public TopicModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public async Task<Topic> Build(TopicCriteria criteria)
        {
            return (await _busMonitor.GetOverview(criteria.RequireFresh)).Topics.Single(t => String.Equals(t.Name, criteria.Topic, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
