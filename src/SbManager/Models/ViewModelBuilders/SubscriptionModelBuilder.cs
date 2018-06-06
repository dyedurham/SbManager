using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class SubscriptionCriteria
    {
        public SubscriptionCriteria(string tid, string sid)
        {
            Topic = tid;
            Subscription = sid;
        }
        public string Topic { get; set; }
        public string Subscription { get; set; }
    }

    public class SubscriptionModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Subscription, SubscriptionCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public SubscriptionModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public Task<Subscription> Build(SubscriptionCriteria criteria)
        {
            return _busMonitor.GetSubscription(criteria.Topic, criteria.Subscription);
        }
    }
}
