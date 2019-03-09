using System.Linq;
using System.Threading.Tasks;
using SbManager.BusHelpers;
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

        public async Task<MessageView> Build(FindQueuedMessages criteria)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(criteria.Path);
            var resp = new MessageView();
            var msgs = await receiver.PeekAsync(criteria.Count);

            foreach (var m in msgs)
            {
                //be dumb so stack traces are awesomer
                var msg = new Message();

                msg.MessageId = m.MessageId;
                msg.CorrelationId = m.CorrelationId;
                msg.Enqueued = m.SystemProperties.EnqueuedTimeUtc;
                msg.Expires = m.ExpiresAtUtc;
                msg.Label = m.Label;
                msg.Sequence = m.SystemProperties.SequenceNumber;
                msg.EnqueuedSequence = m.SystemProperties.EnqueuedSequenceNumber;
                msg.SizeInBytes = m.Size;
                msg.ReplyTo = m.ReplyTo;
                msg.ReplyToSessionId = m.ReplyToSessionId;
                msg.To = m.To;
                msg.SessionId = m.SessionId;
                if (criteria.HaveLock) msg.LockToken = m.SystemProperties.LockToken;
                if (criteria.HaveLock) msg.LockedUntil = m.SystemProperties.LockedUntilUtc;
                msg.TimeToLive = m.TimeToLive;
                msg.ScheduledEnqueueTime = m.ScheduledEnqueueTimeUtc;
                msg.ContentType = m.ContentType;
                msg.DeliveryCount = m.SystemProperties.DeliveryCount;
                msg.CustomProperties = m.UserProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value == null ? null : kvp.Value.ToString());
                msg.Body = m.GetBodyString();

                resp.Messages.Add(msg);
            }
            return resp;
        }

    }
}
