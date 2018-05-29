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

        private Task<Overview> Fetch()
        {
            var overview = new Overview
            {
                Queues = _namespaceManager.GetQueues().Select(e => new Queue
                {
                    Status = e.Status.ToString(),
                    ActiveMessageCount = e.CountDetails.ActiveMessageCount,
                    DeadLetterCount = e.CountDetails.DeadLetterMessageCount,
                    ScheduledMessageCount = e.CountDetails.ScheduledMessageCount,
                    TransferMessageCount = e.CountDetails.TransferMessageCount,
                    DeadTransferMessageCount = e.CountDetails.TransferDeadLetterMessageCount,
                    SizeInBytes = e.SizeInBytes,
                    AutoDeleteOnIdle = new Time(e.AutoDeleteOnIdle),
                    DefaultMessageTTL = new Time(e.DefaultMessageTimeToLive),
                    DuplicateDetectionWindow = new Time(e.DuplicateDetectionHistoryTimeWindow),
                    LockDuration = new Time(e.LockDuration),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    AccessedAt = e.AccessedAt,
                    Name = e.Path
                }).ToList(),
                Topics = _namespaceManager.GetTopics().Select(e => new Topic
                {
                    Status = e.Status.ToString(),
                    Name = e.Path,
                    SizeInBytes = e.SizeInBytes,
                    AutoDeleteOnIdle = new Time(e.AutoDeleteOnIdle),
                    DefaultMessageTTL = new Time(e.DefaultMessageTimeToLive),
                    DuplicateDetectionWindow = new Time(e.DuplicateDetectionHistoryTimeWindow),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    //AccessedAt = e.AccessedAt,
                }).ToList(),
            };

            foreach (var topic in overview.Topics)
            {
                topic.Subscriptions = _namespaceManager.GetSubscriptions(topic.Name).Select(e => new Subscription
                {
                    Status = e.Status.ToString(),
                    Name = e.Name,
                    TopicName = topic.Name,
                    //ActiveMessageCount = e.CountDetails.ActiveMessageCount,
                    //DeadLetterCount = e.CountDetails.DeadLetterMessageCount,
                    //ScheduledMessageCount = e.CountDetails.ScheduledMessageCount,
                    //TransferMessageCount = e.CountDetails.TransferMessageCount,
                    //DeadTransferMessageCount = e.CountDetails.TransferDeadLetterMessageCount,
                    AutoDeleteOnIdle = new Time(e.AutoDeleteOnIdle),
                    DefaultMessageTTL = new Time(e.DefaultMessageTimeToLive),
                    LockDuration = new Time(e.LockDuration),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    AccessedAt = e.AccessedAt,
                }).ToList();

                foreach (var subscription in topic.Subscriptions)
                {
                    subscription.Rules = _namespaceManager.GetRules(topic.Name, subscription.Name).Select(MapRule).ToList();
                }

                topic.ActiveMessageCount = topic.Subscriptions.Sum(s => s.ActiveMessageCount);
                topic.DeadLetterCount = topic.Subscriptions.Sum(s => s.DeadLetterCount);
                topic.ScheduledMessageCount = topic.Subscriptions.Sum(s => s.ScheduledMessageCount);
                topic.TransferMessageCount = topic.Subscriptions.Sum(s => s.TransferMessageCount);
                topic.DeadTransferMessageCount = topic.Subscriptions.Sum(s => s.DeadTransferMessageCount);
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

        private Rule MapRule(RuleDescription r)
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
