using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using SbManager.Extensions;
using BrokeredMessage = Microsoft.Azure.ServiceBus.Message;
using static Microsoft.Azure.ServiceBus.EntityNameHelper;

namespace SbManager.BusHelpers
{
    public interface IRequeueAndRemove
    {
        Task RequeueAll(string path);
        Task RequeueAll(string topicPath, string subscriptionName);
        Task RequeueOne(string path, string messageId, string newBody);
        Task RequeueOne(string topicPath, string subscriptionName, string messageId, string newBody);

        Task RemoveAll(string path);
        Task RemoveAll(string topicPath, string subscriptionName);
        Task RemoveOne(string path, string messageId);
        Task RemoveOne(string topicPath, string subscriptionName, string messageId);

        Task KillAll(string path);
        Task KillAll(string topicPath, string subscriptionName);
        Task KillOne(string path, string messageId);
        Task KillOne(string topicPath, string subscriptionName, string messageId);
    }
    public class RequeueAndRemove : IRequeueAndRemove
    {
        private readonly MessagingFactory _messagingFactory;

        public RequeueAndRemove(MessagingFactory messagingFactory)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task RequeueAll(string path)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(FormatDeadLetterPath(path));
            var sender = _messagingFactory.CreateMessageSender(path);

            // TODO: parallelize this loop?
            Message msg;
            while ((msg = await receiver.ReceiveAsync(TimeSpan.FromSeconds(5))) != null)
            {
                var clone = Clone(msg);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.UserProperties.ContainsKey("RequeuedFrom")) clone.UserProperties["RequeuedFrom"] = clone.UserProperties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.UserProperties.Add("RequeuedFrom", msg.MessageId);

                await sender.SendAsync(clone);
                await receiver.CompleteAsync(msg.SystemProperties.LockToken);
            }
        }

        public Task RequeueAll(string topicPath, string subscriptionName)
        {
            return RequeueAll(FormatSubscriptionPath(topicPath, subscriptionName));
        }

        public async Task RequeueOne(string path, string messageId, string newBody)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(path);
            var sender = _messagingFactory.CreateMessageSender(path);

            // TODO: parallelize this loop
            Message msg;
            while ((msg = await receiver.ReceiveAsync(TimeSpan.FromSeconds(5))) != null)
            {
                if (msg.MessageId != messageId)
                {
                    await receiver.AbandonAsync(msg.SystemProperties.LockToken);
                    continue;
                }
                var clone = Clone(msg, newBody);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.UserProperties.ContainsKey("RequeuedFrom")) clone.UserProperties["RequeuedFrom"] = clone.UserProperties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.UserProperties.Add("RequeuedFrom", msg.MessageId);

                await receiver.CompleteAsync(msg.SystemProperties.LockToken);
                await sender.SendAsync(clone);
            }
        }

        public Task RequeueOne(string topicPath, string subscriptionName, string messageId, string newBody)
        {
            return RequeueOne(FormatSubscriptionPath(topicPath, subscriptionName), messageId, newBody);
        }

        public Task RemoveAll(string path)
        {
            return PerformAll(path, async (receiver, lockToken) => await receiver.CompleteAsync(lockToken));
        }

        public Task RemoveAll(string topicPath, string subscriptionName)
        {
            return RemoveAll(FormatSubscriptionPath(topicPath, subscriptionName));
        }

        public Task RemoveOne(string path, string messageId)
        {
            return PerformOne(path, messageId, async (receiver, lockToken) => await receiver.CompleteAsync(lockToken));
        }

        public Task RemoveOne(string topicPath, string subscriptionName, string messageId)
        {
            return RemoveOne(FormatSubscriptionPath(topicPath, subscriptionName), messageId);
        }

        public Task KillAll(string path)
        {
            return PerformAll(path, async (receiver, lockToken) => await receiver.DeadLetterAsync(lockToken));
        }

        public Task KillAll(string topicPath, string subscriptionName)
        {
            return KillAll(FormatSubscriptionPath(topicPath, subscriptionName));
        }
        
        public Task KillOne(string path, string messageId)
        {
            return PerformOne(path, messageId, async (receiver, lockToken) => await receiver.DeadLetterAsync(lockToken));
        }

        public Task KillOne(string topicPath, string subscriptionName, string messageId)
        {
            return KillOne(FormatSubscriptionPath(topicPath, subscriptionName), messageId);
        }

        public virtual string[] GetPropertiesToRemove()
        {
            return new []{"ExceptionType", "ExceptionMessage", "ExceptionStackTrace", "ExceptionTimestamp", "ExceptionMachineName", "ExceptionIdentityName", "DeadLetterReason", "DeadLetterErrorDescription"};
        }

        private async Task PerformAll(string path, Func<MessageReceiver, string, Task> action)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(path);
            
            // TODO: parallelize this loop?
            Message msg;
            while ((msg = await receiver.ReceiveAsync(TimeSpan.FromSeconds(5))) != null)
            {
                await action(receiver, msg.SystemProperties.LockToken);
            }
        }

        private async Task PerformOne(string path, string messageId, Func<MessageReceiver, string, Task> action)
        {
            var receiver = _messagingFactory.CreateMessageReceiver(path);
            
            // TODO: parallelize this loop?
            Message msg;
            while ((msg = await receiver.ReceiveAsync(TimeSpan.FromSeconds(5))) != null)
            {
                if (msg.MessageId != messageId)
                {
                    await receiver.AbandonAsync(msg.SystemProperties.LockToken);
                    continue;
                }
                await action(receiver, msg.SystemProperties.LockToken);
            }
        }
        
        private BrokeredMessage Clone(BrokeredMessage msg, string newBody = null)
        {
            if (string.IsNullOrWhiteSpace(newBody)) return msg.Clone();

            var cloned = new BrokeredMessage(Encoding.UTF8.GetBytes(newBody))
            {
                Label = msg.Label,
                ContentType = msg.ContentType,
                MessageId = msg.MessageId,
                SessionId = msg.SessionId,
                CorrelationId = msg.CorrelationId,
                To = msg.To,
                ReplyTo = msg.ReplyTo,
                ReplyToSessionId = msg.ReplyToSessionId,
                TimeToLive = msg.TimeToLive,
                ScheduledEnqueueTimeUtc = msg.ScheduledEnqueueTimeUtc,
            };

            foreach (var property in msg.UserProperties)
            {
                cloned.UserProperties.Add(property.Key, property.Value);
            }
            return cloned;
        }
    }
}
