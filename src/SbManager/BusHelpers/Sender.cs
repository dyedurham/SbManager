using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using SbManager.Extensions;
using SbManager.Models.ViewModels;

namespace SbManager.BusHelpers
{
    public interface ISender
    {
        void Send(Message message, string queuePath);
        void Publish(Message message, string topicPath);
    }

    public class Sender : ISender
    {
        private readonly MessagingFactory _messagingFactory;

        public Sender(MessagingFactory messagingFactory)
        {
            _messagingFactory = messagingFactory;
        }

        public void Send(Message message, string queuePath)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var sender = client.MessagingFactory.CreateMessageSender(client.Path);
            sender.Send(Map(message));
        }

        public void Publish(Message message, string topicPath)
        {
            var client = _messagingFactory.CreateTopicClient(topicPath);
            var sender = client.MessagingFactory.CreateMessageSender(client.Path);
            sender.Send(Map(message));
        }

        private BrokeredMessage Map(Message message)
        {
            var brokered = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(message.Body)), true);
            if (message.Label.HasValue()) brokered.Label = message.Label;
            if (message.ContentType.HasValue()) brokered.ContentType = message.ContentType;
            if (message.MessageId.HasValue()) brokered.MessageId = message.MessageId;
            if (message.SessionId.HasValue()) brokered.SessionId = message.SessionId;
            if (message.CorrelationId.HasValue()) brokered.CorrelationId = message.CorrelationId;
            if (message.To.HasValue()) brokered.To = message.To;
            if (message.ReplyTo.HasValue()) brokered.ReplyTo = message.ReplyTo;
            if (message.ReplyToSessionId.HasValue()) brokered.ReplyToSessionId = message.ReplyToSessionId;
            if (message.TimeToLive.HasValue()) brokered.TimeToLive = message.TimeToLive;
            if (message.ScheduledEnqueueTime.HasValue()) brokered.ScheduledEnqueueTimeUtc = message.ScheduledEnqueueTime;

            if (message.CustomProperties != null)
            {
                foreach (var property in message.CustomProperties)
                {
                    brokered.Properties.Add(property.Key, property.Value);
                }
            }

            return brokered;
        }
    }
}
