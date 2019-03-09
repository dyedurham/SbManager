using System.Linq;
using System.Threading.Tasks;
using SbManager.BusHelpers;
using SbManager.CQRS.ModelBuilders;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class DeadletterViewBuilder : IModelBuilder<DeadletterView>
    {
        private readonly IBusMonitor _busMonitor;
        private readonly string _baseUrl;

        public DeadletterViewBuilder(IBusMonitor busMonitor, IConfig config)
        {
            _baseUrl = config.WebAppUrl;
            _busMonitor = busMonitor;
        }

        public async Task<DeadletterView> Build()
        {
            var overview = await _busMonitor.GetOverview(true);
            var deadletterQueues = overview.Queues.Where(q => q.DeadLetterCount > 0)
                .Select(q => new Deadletter
                {
                    DeadLetterCount = q.DeadLetterCount,
                    Name = q.Name,
                    Href = CreateQueueUrl(q.Name)
                });
            var deadletterSubscription = overview.Topics.SelectMany(t => t.Subscriptions)
                .Where(s => s.DeadLetterCount > 0).Select(s => new Deadletter
                {
                    DeadLetterCount = s.DeadLetterCount,
                    Name = s.Name,
                    Href = CreateSubscriptionUrl(s.TopicName, s.Name)
                });

            return new DeadletterView
            {
                Deadletters = deadletterQueues.Concat(deadletterSubscription).ToList()
            };
        }
        private string CreateSubscriptionUrl(string topicName, string subName)
        {
            return _baseUrl + "/#/topic/" + topicName + "/" + subName;
        }


        private string CreateQueueUrl(string name)
        {
            return _baseUrl + "/#/queue/" + name;
        }
    }
}
