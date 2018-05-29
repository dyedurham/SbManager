using System.Text;
using System.Threading.Tasks;
using SbManager.Extensions;
using SbManager.Models.ViewModels;
using BrokeredMessage = Microsoft.Azure.ServiceBus.Message;

namespace SbManager.BusHelpers
{
    public interface ISender
    {
        Task Send(Message message, string queuePath);
        Task Publish(Message message, string topicPath);
    }

    public class Sender : ISender
    {
        private readonly MessagingFactory _messagingFactory;

        public Sender(MessagingFactory messagingFactory)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task Send(Message message, string queuePath)
        {
            var sender = _messagingFactory.CreateMessageSender(queuePath);
            await sender.SendAsync(Map(message));
        }

        public async Task Publish(Message message, string topicPath)
        {
            var sender = _messagingFactory.CreateMessageSender(topicPath);
            await sender.SendAsync(Map(message));
        }

        private BrokeredMessage Map(Message message)
        {
            var brokered = new BrokeredMessage(Encoding.UTF8.GetBytes(message.Body));
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
                    brokered.UserProperties.Add(property.Key, property.Value);
                }
            }

            return brokered;
        }
    }
}
