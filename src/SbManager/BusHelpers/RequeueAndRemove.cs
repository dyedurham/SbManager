using System;
using System.IO;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SbManager.Extensions;

namespace SbManager.BusHelpers
{
    public interface IRequeueAndRemove
    {
        void RequeueAll(string queuePath);
        void RequeueAll(string topicPath, string subscriptionName);
        void RequeueOne(string queuePath, string messageId, string newBody);
        void RequeueOne(string topicPath, string subscriptionName, string messageId, string newBody);

        void RemoveAll(string queuePath);
        void RemoveAll(string topicPath, string subscriptionName);
        void RemoveOne(string queuePath, string messageId);
        void RemoveOne(string topicPath, string subscriptionName, string messageId);

        void Kill(string queuePath, string messageId);
        void Kill(string topicPath, string subscriptionName, string messageId);
        void KillAll(string queuePath);
        void KillAll(string topicPath, string subscriptionName);
    }
    public class RequeueAndRemove : IRequeueAndRemove
    {
        private readonly MessagingFactory _messagingFactory;
        private readonly NamespaceManager _namespaceManager;

        public RequeueAndRemove(MessagingFactory messagingFactory, NamespaceManager namespaceManager)
        {
            _messagingFactory = messagingFactory;
            _namespaceManager = namespaceManager;
        }

        public void RequeueAll(string queuePath)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath.MakeDeadLetterPath());
            var queue = _namespaceManager.GetQueue(queuePath);
            var count = queue.MessageCountDetails.DeadLetterMessageCount;
            var sender = client.MessagingFactory.CreateMessageSender(queuePath);

            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                var clone = Clone(msg);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.Properties.ContainsKey("RequeuedFrom")) clone.Properties["RequeuedFrom"] = clone.Properties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.Properties.Add("RequeuedFrom", msg.MessageId);

                sender.Send(clone);
                msg.Complete();
            }
        }

        public void RequeueAll(string topicPath, string subscriptionName)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName.MakeDeadLetterPath());
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName);
            var count = sub.MessageCountDetails.DeadLetterMessageCount;
            var sender = client.MessagingFactory.CreateMessageSender(client.TopicPath);

            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                var clone = Clone(msg);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.Properties.ContainsKey("RequeuedFrom")) clone.Properties["RequeuedFrom"] = clone.Properties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.Properties.Add("RequeuedFrom", msg.MessageId);

                sender.Send(clone);
                msg.Complete();
            }
        }

        public void RequeueOne(string queuePath, string messageId, string newBody)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var queue = _namespaceManager.GetQueue(queuePath.RemoveDeadLetterPath());
            var count = GetQueueMessageCount(queuePath, queue);
            var sender = _messagingFactory.CreateMessageSender(queuePath.RemoveDeadLetterPath());

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                var clone = Clone(msg, newBody);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.Properties.ContainsKey("RequeuedFrom")) clone.Properties["RequeuedFrom"] = clone.Properties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.Properties.Add("RequeuedFrom", msg.MessageId);

                msg.Complete();
                sender.Send(clone);
            }
        }

        public void RequeueOne(string topicPath, string subscriptionName, string messageId, string newBody)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName);
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName.RemoveDeadLetterPath());
            var count = GetSubscriptionMessageCount(subscriptionName, sub);
            var sender = client.MessagingFactory.CreateMessageSender(client.TopicPath);

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                var clone = Clone(msg, newBody);
                clone.RemoveProperties(GetPropertiesToRemove());

                if (clone.Properties.ContainsKey("RequeuedFrom")) clone.Properties["RequeuedFrom"] = clone.Properties["RequeuedFrom"] += "," + msg.MessageId;
                else clone.Properties.Add("RequeuedFrom", msg.MessageId);

                sender.Send(clone);
                msg.Complete();
            }
        }

        public void RemoveAll(string queuePath)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var queue = _namespaceManager.GetQueue(queuePath.RemoveDeadLetterPath());
            var count = GetQueueMessageCount(queuePath, queue);

            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                msg.Complete();
            }
        }

        public void RemoveAll(string topicPath, string subscriptionName)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName);
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName.RemoveDeadLetterPath());
            var count = GetSubscriptionMessageCount(subscriptionName, sub);

            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                msg.Complete();
            }
        }

        public void RemoveOne(string queuePath, string messageId)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var queue = _namespaceManager.GetQueue(queuePath.RemoveDeadLetterPath());
            var count = GetQueueMessageCount(queuePath, queue);

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                msg.Complete();
            }
        }

        public void RemoveOne(string topicPath, string subscriptionName, string messageId)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName);
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName.RemoveDeadLetterPath());
            var count = GetSubscriptionMessageCount(subscriptionName, sub);

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                msg.Complete();
            }
        }

        public void Kill(string queuePath, string messageId)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var queue = _namespaceManager.GetQueue(queuePath.RemoveDeadLetterPath());
            var count = GetQueueMessageCount(queuePath, queue);

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                msg.DeadLetter();
                return;
            }
        }

        public void Kill(string topicPath, string subscriptionName, string messageId)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName);
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName.RemoveDeadLetterPath());
            var count = GetSubscriptionMessageCount(subscriptionName, sub);

            var msgs = client.ReceiveBatch(Convert.ToInt32(count));
            foreach (var msg in msgs)
            {
                if (msg.MessageId != messageId)
                {
                    msg.Abandon();
                    continue;
                }
                msg.DeadLetter();
                return;
            }
        }

        public void KillAll(string queuePath)
        {
            var client = _messagingFactory.CreateQueueClient(queuePath);
            var queue = _namespaceManager.GetQueue(queuePath.RemoveDeadLetterPath());
            var count = GetQueueMessageCount(queuePath, queue);


            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                msg.DeadLetter();
            }
        }

        public void KillAll(string topicPath, string subscriptionName)
        {
            var client = _messagingFactory.CreateSubscriptionClient(topicPath, subscriptionName);
            var sub = _namespaceManager.GetSubscription(topicPath, subscriptionName.RemoveDeadLetterPath());
            var count = GetSubscriptionMessageCount(subscriptionName, sub);

            for (var i = 0; i < count; i++)
            {
                var msg = client.Receive(new TimeSpan(0, 0, 5));
                if (msg == null) break;
                msg.DeadLetter();
            }
        }

        public virtual string[] GetPropertiesToRemove()
        {
            return new []{"ExceptionType", "ExceptionMessage", "ExceptionStackTrace", "ExceptionTimestamp", "ExceptionMachineName", "ExceptionIdentityName", "DeadLetterReason", "DeadLetterErrorDescription"};
        }

        private BrokeredMessage Clone(BrokeredMessage msg, string newBody = null)
        {
            if (string.IsNullOrWhiteSpace(newBody)) return msg.Clone();

            var cloned = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(newBody)), true)
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

            foreach (var property in msg.Properties)
            {
                cloned.Properties.Add(property.Key, property.Value);
            }
            return cloned;
        }

        private static long GetQueueMessageCount(string queuePath, QueueDescription queue)
        {
            return queuePath.IsDeadLetterPath() ? queue.MessageCountDetails.DeadLetterMessageCount : (queue.MessageCountDetails.ActiveMessageCount + queue.MessageCountDetails.ScheduledMessageCount);
        }

        private static long GetSubscriptionMessageCount(string subscriptionName, SubscriptionDescription sub)
        {
            return subscriptionName.IsDeadLetterPath() ? sub.MessageCountDetails.DeadLetterMessageCount : (sub.MessageCountDetails.ActiveMessageCount + sub.MessageCountDetails.ScheduledMessageCount);
        }
    }
}
