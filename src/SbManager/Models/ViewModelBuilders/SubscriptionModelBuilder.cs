using System;
using System.Linq;
using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class SubscriptionCriteria
    {
        public SubscriptionCriteria(string tid, string sid, bool requireFresh = false)
        {
            Topic = tid;
            Subscription = sid;
            RequireFresh = requireFresh;
        }
        public string Topic { get; set; }
        public string Subscription { get; set; }
        public bool RequireFresh { get; set; }
    }

    public class SubscriptionModelBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<Subscription, SubscriptionCriteria>
    {
        private readonly IBusMonitor _busMonitor;

        public SubscriptionModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }

        public Subscription Build(SubscriptionCriteria criteria)
        {
            return _busMonitor.GetOverview(criteria.RequireFresh)
                .Topics.Single(t => String.Equals(t.Name, criteria.Topic, StringComparison.CurrentCultureIgnoreCase))
                .Subscriptions.Single(s => String.Equals(s.Name, criteria.Subscription, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
