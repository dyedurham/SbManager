using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using SbManager.Models.ViewModels;

namespace SbManager.BusHelpers
{
    public interface IBusMonitor
    {
        Overview GetOverview(bool fresh = false);
    }
    public class BusMonitor : IBusMonitor
    {
        private readonly IConfig _config;
        private const long RefreshTime = 5000;
        private DateTime _lastTouch = new DateTime(1, 1, 1);
        private Overview _cached;
        private readonly object _lock = new { };
        public Func<DateTime> GetTime = () => DateTime.Now;

        public BusMonitor(IConfig config)
        {
            _config = config;
        }

        public Overview GetOverview(bool forceDirty = false)
        {
            if (!Dirty(forceDirty)) return _cached;

            lock (_lock)
            {
                if (Dirty(forceDirty)) _cached = Fetch();
            }

            return _cached;
        }

        private Overview Fetch()
        {
            var manager = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(_config.BusConnectionString);
            var overview = new Overview
            {
                Queues = manager.GetQueues().Select(e => new Queue
                {
                    Status = e.Status.ToString(),
                    ActiveMessageCount = e.MessageCountDetails.ActiveMessageCount,
                    DeadLetterCount = e.MessageCountDetails.DeadLetterMessageCount,
                    ScheduledMessageCount = e.MessageCountDetails.ScheduledMessageCount,
                    TransferMessageCount = e.MessageCountDetails.TransferMessageCount,
                    DeadTransferMessageCount = e.MessageCountDetails.TransferDeadLetterMessageCount,
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
                Topics = manager.GetTopics().Select(e => new Topic
                {
                    Status = e.Status.ToString(),
                    Name = e.Path,
                    SizeInBytes = e.SizeInBytes,
                    AutoDeleteOnIdle = new Time(e.AutoDeleteOnIdle),
                    DefaultMessageTTL = new Time(e.DefaultMessageTimeToLive),
                    DuplicateDetectionWindow = new Time(e.DuplicateDetectionHistoryTimeWindow),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    AccessedAt = e.AccessedAt,
                }).ToList(),
            };

            foreach (var topic in overview.Topics)
            {
                topic.Subscriptions = manager.GetSubscriptions(topic.Name).Select(e => new Subscription
                {
                    Status = e.Status.ToString(),
                    Name = e.Name,
                    TopicName = topic.Name,
                    ActiveMessageCount = e.MessageCountDetails.ActiveMessageCount,
                    DeadLetterCount = e.MessageCountDetails.DeadLetterMessageCount,
                    ScheduledMessageCount = e.MessageCountDetails.ScheduledMessageCount,
                    TransferMessageCount = e.MessageCountDetails.TransferMessageCount,
                    DeadTransferMessageCount = e.MessageCountDetails.TransferDeadLetterMessageCount,
                    AutoDeleteOnIdle = new Time(e.AutoDeleteOnIdle),
                    DefaultMessageTTL = new Time(e.DefaultMessageTimeToLive),
                    LockDuration = new Time(e.LockDuration),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    AccessedAt = e.AccessedAt,
                    Rules = manager.GetRules(topic.Name, e.Name).Select(MapRule).ToList()
                }).ToList();

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
