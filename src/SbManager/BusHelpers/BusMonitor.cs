using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
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
        private readonly ManagementClient _managementClient;
        private const long RefreshTime = 5000;
        private DateTime _lastTouch = new DateTime(1, 1, 1);
        private Overview _cached;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public Func<DateTime> GetTime = () => DateTime.Now;

        public BusMonitor(ManagementClient managementClient)
        {
            _managementClient = managementClient;
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

        public async Task<Queue> GetQueue(string queueName)
        {
            var queueDescription = await _managementClient.GetQueueAsync(queueName);
            var queueInfo = await _managementClient.GetQueueRuntimeInfoAsync(queueName);
            var queue = MapQueue(queueDescription, queueInfo);
            return queue;
        }

        public async Task<Topic> GetTopic(string topicName)
        {
            var topicDescription = await _managementClient.GetTopicAsync(topicName);
            var topicInfo = await _managementClient.GetTopicRuntimeInfoAsync(topicName);
            var topic = MapTopic(topicDescription, topicInfo);
            await FillTopic(topic);
            return topic;
        }

        public async Task<Subscription> GetSubscription(string topicName, string subscriptionName)
        {
            var subscriptionDescription = await _managementClient.GetSubscriptionAsync(topicName, subscriptionName);
            var subscriptionInfo = await _managementClient.GetSubscriptionRuntimeInfoAsync(topicName, subscriptionName);
            var subscription = MapSubscription(topicName, subscriptionDescription, subscriptionInfo);
            await FillSubscription(subscription, topicName);
            return subscription;
        }

        private async Task FillTopic(Topic topic)
        {
            var subscriptions = await _managementClient.GetSubscriptionsAsync(topic.Name);
            var subscriptionsRuntimeInfoTasks = subscriptions.Select(e => _managementClient.GetSubscriptionRuntimeInfoAsync(topic.Name, e.SubscriptionName)).ToList();
            await Task.WhenAll(subscriptionsRuntimeInfoTasks);
            var subscriptionsRuntimeInfos = subscriptionsRuntimeInfoTasks.Select(e => e.Result);
            
            topic.Subscriptions = subscriptionsRuntimeInfos.Select(e => MapSubscription(topic.Name, subscriptions.Single(s => s.SubscriptionName == e.SubscriptionName), e)).ToList();

            await Task.WhenAll(topic.Subscriptions.Select(e => FillSubscription(e, topic.Name)));

            topic.ActiveMessageCount = topic.Subscriptions.Sum(s => s.ActiveMessageCount);
            topic.DeadLetterCount = topic.Subscriptions.Sum(s => s.DeadLetterCount);
            topic.ScheduledMessageCount = topic.Subscriptions.Sum(s => s.ScheduledMessageCount);
            topic.TransferMessageCount = topic.Subscriptions.Sum(s => s.TransferMessageCount);
            topic.DeadTransferMessageCount = topic.Subscriptions.Sum(s => s.DeadTransferMessageCount);
        }

        private async Task FillSubscription(Subscription subscription, string topicName)
        {
            subscription.Rules = (await _managementClient.GetRulesAsync(topicName, subscription.Name)).Select(MapRule).ToList();
        }

        private async Task<Overview> Fetch()
        {
            var queues = await _managementClient.GetQueuesAsync();
            var queuesRuntimeInfoTasks = queues.Select(e => _managementClient.GetQueueRuntimeInfoAsync(e.Path)).ToList();
            await Task.WhenAll(queuesRuntimeInfoTasks);
            var queueRuntimeInfos = queuesRuntimeInfoTasks.Select(e => e.Result);

            var topics = await _managementClient.GetTopicsAsync();
            var topicsRuntimeInfoTasks = topics.Select(e => _managementClient.GetTopicRuntimeInfoAsync(e.Path)).ToList();
            await Task.WhenAll(topicsRuntimeInfoTasks);
            var topicsRuntimeInfos = topicsRuntimeInfoTasks.Select(e => e.Result);
            
            var overview = new Overview
            {
                Queues = queueRuntimeInfos.Select(e => MapQueue(queues.Single(q => q.Path == e.Path), e)).ToList(),
                Topics = topicsRuntimeInfos.Select(e => MapTopic(topics.Single(t => t.Path == e.Path), e)).ToList()
            };

            await Task.WhenAll(overview.Topics.Select(FillTopic));

            var queueMessageCounts = GetCounts(overview.Queues);
            var topicMessageCounts = GetCounts(overview.Topics);

            overview.TotalDeadLetters = queueMessageCounts.DeadLetterCount + topicMessageCounts.DeadLetterCount;
            overview.TotalActiveMessages = queueMessageCounts.ActiveMessageCount + topicMessageCounts.ActiveMessageCount;
            overview.TotalScheduledMessages = queueMessageCounts.ScheduledMessageCount + topicMessageCounts.ScheduledMessageCount;

            return overview;
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

        private static Queue MapQueue(QueueDescription queueDescription, QueueRuntimeInfo queueInfo)
        {
            return new Queue
            {
                Status = queueDescription.Status.ToString(),
                ActiveMessageCount = queueInfo.MessageCountDetails.ActiveMessageCount,
                DeadLetterCount = queueInfo.MessageCountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = queueInfo.MessageCountDetails.ScheduledMessageCount,
                TransferMessageCount = queueInfo.MessageCountDetails.TransferMessageCount,
                DeadTransferMessageCount = queueInfo.MessageCountDetails.TransferDeadLetterMessageCount,
                SizeInBytes = queueInfo.SizeInBytes,
                AutoDeleteOnIdle = new Time(queueDescription.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(queueDescription.DefaultMessageTimeToLive),
                DuplicateDetectionWindow = new Time(queueDescription.DuplicateDetectionHistoryTimeWindow),
                LockDuration = new Time(queueDescription.LockDuration),
                CreatedAt = queueInfo.CreatedAt,
                UpdatedAt = queueInfo.UpdatedAt,
                AccessedAt = queueInfo.AccessedAt,
                Name = queueDescription.Path
            };
        }

        private static Topic MapTopic(TopicDescription topicDescription, TopicRuntimeInfo topicInfo)
        {
            return new Topic
            {
                Status = topicDescription.Status.ToString(),
                Name = topicDescription.Path,
                SizeInBytes = topicInfo.SizeInBytes,
                AutoDeleteOnIdle = new Time(topicDescription.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(topicDescription.DefaultMessageTimeToLive),
                DuplicateDetectionWindow = new Time(topicDescription.DuplicateDetectionHistoryTimeWindow),
                CreatedAt = topicInfo.CreatedAt,
                UpdatedAt = topicInfo.UpdatedAt,
                AccessedAt = topicInfo.AccessedAt,
            };
        }
        
        private static Subscription MapSubscription(string topicName, SubscriptionDescription subscriptionDescription, SubscriptionRuntimeInfo subscriptionInfo)
        {
            return new Subscription
            {
                Status = subscriptionDescription.Status.ToString(),
                Name = subscriptionDescription.SubscriptionName,
                TopicName = topicName,
                ActiveMessageCount = subscriptionInfo.MessageCountDetails.ActiveMessageCount,
                DeadLetterCount = subscriptionInfo.MessageCountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = subscriptionInfo.MessageCountDetails.ScheduledMessageCount,
                TransferMessageCount = subscriptionInfo.MessageCountDetails.TransferMessageCount,
                DeadTransferMessageCount = subscriptionInfo.MessageCountDetails.TransferDeadLetterMessageCount,
                AutoDeleteOnIdle = new Time(subscriptionDescription.AutoDeleteOnIdle),
                DefaultMessageTTL = new Time(subscriptionDescription.DefaultMessageTimeToLive),
                LockDuration = new Time(subscriptionDescription.LockDuration),
                CreatedAt = subscriptionInfo.CreatedAt,
                UpdatedAt = subscriptionInfo.UpdatedAt,
                AccessedAt = subscriptionInfo.AccessedAt,
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
                    //CreatedAt = r.CreatedAt
                };
            }
            if (r.Filter is CorrelationFilter)
            {
                return new Rule
                {
                    Name = r.Name,
                    FilterType = "CorrelationFilter",
                    Text = "TODO",
                    //CreatedAt = r.CreatedAt
                };
            }
            return new Rule
            {
                Name = r.Name,
                //CreatedAt = r.CreatedAt,
                Text = "Unknown",
                FilterType = "UnknownFilterType"
            };
        }
    }
}
