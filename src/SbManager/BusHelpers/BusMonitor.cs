using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mossharbor.AzureWorkArounds.ServiceBus;
using SbManager.Models.ViewModels;

namespace SbManager.BusHelpers
{
    public interface IBusMonitor
    {
        Task<Overview> GetOverview(bool fresh = false);
        Task<Queue> GetQueue(string queueName);
        Task<Topic> GetTopic(string topicName);
        Task<Subscription> GetSubscription(string topicName, string subscriptionName);
    }
    public class BusMonitor : IBusMonitor
    {
        private readonly NamespaceManager _namespaceManager;
        private const long RefreshTime = 5000;
        private DateTime _lastTouch = new DateTime(1, 1, 1);
        private Overview _cached;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public Func<DateTime> GetTime = () => DateTime.Now;

        public BusMonitor(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager;
        }

        public async Task<Overview> GetOverview(bool forceDirty = false)
        {
            if (!Dirty(forceDirty)) return _cached;

            await _semaphore.WaitAsync();
            try
            {
                if (Dirty(forceDirty)) _cached = await Fetch();
            }
            finally
            {
                _semaphore.Release();
            }

            return _cached;
        }

        public Task<Queue> GetQueue(string queueName)
        {
            var queue = MapQueue(_namespaceManager.GetQueue(queueName));
            return Task.FromResult(queue);
        }

        public Task<Topic> GetTopic(string topicName)
        {
            var topic = MapTopic(_namespaceManager.GetTopic(topicName));
            FillTopic(topic);
            return Task.FromResult(topic);
        }

        public Task<Subscription> GetSubscription(string topicName, string subscriptionName)
        {
            var subscription = MapSubscription(topicName, _namespaceManager.GetSubscription(topicName, subscriptionName));
            FillSubscription(subscription, topicName);
            return Task.FromResult(subscription);
        }

        private void FillTopic(Topic topic)
        {
            topic.Subscriptions = _namespaceManager.GetSubscriptions(topic.Name).Select(e => MapSubscription(topic.Name, e)).ToList();

            foreach (var subscription in topic.Subscriptions)
            {
                FillSubscription(subscription, topic.Name);
            }

            topic.ActiveMessageCount = topic.Subscriptions.Sum(s => s.ActiveMessageCount);
            topic.DeadLetterCount = topic.Subscriptions.Sum(s => s.DeadLetterCount);
            topic.ScheduledMessageCount = topic.Subscriptions.Sum(s => s.ScheduledMessageCount);
            topic.TransferMessageCount = topic.Subscriptions.Sum(s => s.TransferMessageCount);
            topic.DeadTransferMessageCount = topic.Subscriptions.Sum(s => s.DeadTransferMessageCount);
        }

        private void FillSubscription(Subscription subscription, string topicName)
        {
            subscription.Rules = _namespaceManager.GetRules(topicName, subscription.Name).Select(MapRule).ToList();
        }

        private Task<Overview> Fetch()
        {
            var overview = new Overview
            {
                Queues = _namespaceManager.GetQueues().Select(MapQueue).ToList(),
                Topics = _namespaceManager.GetTopics().Select(MapTopic).ToList()
            };

            foreach (var topic in overview.Topics)
            {
                FillTopic(topic);
            }

            var queueMessageCounts = GetCounts(overview.Queues);
            var topicMessageCounts = GetCounts(overview.Topics);

            overview.TotalDeadLetters = queueMessageCounts.DeadLetterCount + topicMessageCounts.DeadLetterCount;
            overview.TotalActiveMessages = queueMessageCounts.ActiveMessageCount + topicMessageCounts.ActiveMessageCount;
            overview.TotalScheduledMessages = queueMessageCounts.ScheduledMessageCount + topicMessageCounts.ScheduledMessageCount;

            return Task.FromResult(overview);
        }

        private class MessageCounts
        {
            public long DeadLetterCount { get; set; }
            public long ActiveMessageCount { get; set; }
            public long ScheduledMessageCount { get; set; }
        }

        private static MessageCounts GetCounts(IEnumerable<IHaveMessageCounts> queueOrTopic)
        {
            var seed = new MessageCounts { DeadLetterCount = 0L, ActiveMessageCount = 0L, ScheduledMessageCount = 0L };

            return queueOrTopic.Aggregate(seed, (_, queue) => new MessageCounts
            {
                DeadLetterCount = _.DeadLetterCount + queue.DeadLetterCount,
                ActiveMessageCount = _.ActiveMessageCount + queue.ActiveMessageCount,
                ScheduledMessageCount = _.ScheduledMessageCount + queue.ScheduledMessageCount
            });
        }

        private bool Dirty(bool forceDirty)
        {
            return forceDirty || (GetTime() > _lastTouch.AddMilliseconds(RefreshTime));
        }

        private static Queue MapQueue(QueueDescription queue)
        {
            return new Queue
            {
                Status = queue.Status.ToString(),
                ActiveMessageCount = queue.CountDetails.ActiveMessageCount,
                DeadLetterCount = queue.CountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = queue.CountDetails.ScheduledMessageCount,
                TransferMessageCount = queue.CountDetails.TransferMessageCount,
                DeadTransferMessageCount = queue.CountDetails.TransferDeadLetterMessageCount,
                SizeInBytes = queue.SizeInBytes,
                AutoDeleteOnIdle = new Time(queue.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(queue.DefaultMessageTimeToLive),
                DuplicateDetectionWindow = new Time(queue.DuplicateDetectionHistoryTimeWindow),
                LockDuration = new Time(queue.LockDuration),
                CreatedAt = queue.CreatedAt,
                UpdatedAt = queue.UpdatedAt,
                AccessedAt = queue.AccessedAt,
                Name = queue.Path
            };
        }

        private static Topic MapTopic(TopicDescription topic)
        {
            return new Topic
            {
                Status = topic.Status.ToString(),
                Name = topic.Path,
                SizeInBytes = topic.SizeInBytes,
                AutoDeleteOnIdle = new Time(topic.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(topic.DefaultMessageTimeToLive),
                DuplicateDetectionWindow = new Time(topic.DuplicateDetectionHistoryTimeWindow),
                CreatedAt = topic.CreatedAt,
                UpdatedAt = topic.UpdatedAt,
                //AccessedAt = topic.AccessedAt,
            };
        }
        
        private static Subscription MapSubscription(string topicName, SubscriptionDescription subscription)
        {
            return new Subscription
            {
                Status = subscription.Status.ToString(),
                Name = subscription.Name,
                TopicName = topicName,
                //ActiveMessageCount = subscription.CountDetails.ActiveMessageCount,
                //DeadLetterCount = subscription.CountDetails.DeadLetterMessageCount,
                //ScheduledMessageCount = subscription.CountDetails.ScheduledMessageCount,
                //TransferMessageCount = subscription.CountDetails.TransferMessageCount,
                //DeadTransferMessageCount = subscription.CountDetails.TransferDeadLetterMessageCount,
                AutoDeleteOnIdle = new Time(subscription.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(subscription.DefaultMessageTimeToLive),
                LockDuration = new Time(subscription.LockDuration),
                CreatedAt = subscription.CreatedAt,
                UpdatedAt = subscription.UpdatedAt,
                AccessedAt = subscription.AccessedAt,
            };
        }

        private static Rule MapRule(RuleDescription r)
        {
            if (r.Filter is SqlFilter)
            {
                return new Rule
                {
                    Name = r.Name,
                    FilterType = "SqlFilter",
                    Text = (r.Filter as SqlFilter).SqlExpression,
                    CreatedAt = r.CreatedAt
                };
            }
            if (r.Filter is CorrelationFilter)
            {
                return new Rule
                {
                    Name = r.Name,
                    FilterType = "CorrelationFilter",
                    Text = "TODO",
                    CreatedAt = r.CreatedAt
                };
            }
            return new Rule
            {
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                Text = "Unknown",
                FilterType = "UnknownFilterType"
            };
        }
    }
}
