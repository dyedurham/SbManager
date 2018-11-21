using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using SbManager.BusHelpers;
using SbManager.Extensions;
using SbManager.Models.ViewModels;
using Message = SbManager.Models.ViewModels.Message;

namespace SbManager.Models.ViewModelBuilders
{
    public class FindSubscriptionMessages
    {
        public FindSubscriptionMessages(string topic, string path, int count, bool haveLock = false)
        {
            Topic = topic;
            Path = path;
            Count = count;
            HaveLock = haveLock;
        }
        public string Topic { get; set; }
        public string Path { get; set; }
        public int Count { get; set; }
        public bool HaveLock { get; set; }
    }
    public class SubscriptionMessagesBuilder : CQRS.ModelBuilders.IModelBuilderWithCriteria<MessageView, FindSubscriptionMessages>
    {
        private readonly MessagingFactory _messagingFactory;

        public SubscriptionMessagesBuilder(MessagingFactory messagingFactory)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task<MessageView> Build(FindSubscriptionMessages criteria)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(EntityNameHelper.FormatSubscriptionPath(criteria.Topic, criteria.Path));
            var resp = new MessageView();
            var msgs = await receiver.PeekAsync(criteria.Count);

            foreach(var m in msgs)
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
