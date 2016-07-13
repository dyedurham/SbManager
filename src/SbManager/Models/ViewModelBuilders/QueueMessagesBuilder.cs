using System;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using SbManager.Extensions;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class FindQueuedMessages
    {
        public FindQueuedMessages(string path, int count, bool haveLock = false)
        {
            Path = path;
            Count = count;
            HaveLock = haveLock;
        }
        public string Path { get; set; }
        public int Count { get; set; }
        public bool HaveLock { get; set; }
    }
    public class QueueMessagesBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<MessageView, FindQueuedMessages>
    {
        private readonly MessagingFactory _messagingFactory;

        public QueueMessagesBuilder(MessagingFactory messagingFactory)
        {
            _messagingFactory = messagingFactory;
        }

        public MessageView Build(FindQueuedMessages criteria)
        {
            var client = _messagingFactory.CreateQueueClient(criteria.Path);
            var resp = new MessageView();
            var msgs = client.PeekBatch(criteria.Count);

            foreach (var m in msgs)
            {
                //be dumb so stack traces are awesomer
                var msg = new Message();

                msg.MessageId = m.MessageId;
                msg.CorrelationId = m.CorrelationId;
                msg.Enqueued = m.EnqueuedTimeUtc;
                msg.Expires = m.ExpiresAtUtc;
                msg.Label = m.Label;
                msg.Sequence = m.SequenceNumber;
                msg.EnqueuedSequence = m.EnqueuedSequenceNumber;
                msg.SizeInBytes = m.Size;
                msg.ReplyTo = m.ReplyTo;
                msg.ReplyToSessionId = m.ReplyToSessionId;
                msg.To = m.To;
                msg.SessionId = m.SessionId;
                msg.State = m.State.ToString();
                if (criteria.HaveLock) msg.LockToken = m.LockToken.ToString();
                if (criteria.HaveLock) msg.LockedUntil = m.LockedUntilUtc;
                msg.TimeToLive = m.TimeToLive;
                msg.ScheduledEnqueueTime = m.ScheduledEnqueueTimeUtc;
                msg.IsScheduled = !DateTime.Equals(m.ScheduledEnqueueTimeUtc, DateTime.MinValue);
                msg.ContentType = m.ContentType;
                msg.IsBodyConsumed = m.IsBodyConsumed;
                msg.DeliveryCount = m.DeliveryCount;
                msg.CustomProperties = m.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value == null ? null : kvp.Value.ToString());
                msg.Body = m.GetBodyString();

                resp.Messages.Add(msg);
            }
            return resp;
        }

    }
}
