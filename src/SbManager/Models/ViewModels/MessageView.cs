using System;
using System.Collections.Generic;

namespace SbManager.Models.ViewModels
{
    public class MessageView
    {
        public MessageView()
        {
            Messages = new List<Message>();
        }
        public List<Message> Messages { get; set; }
    }
    public class Message
    {
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public long Sequence { get; set; }
        public long EnqueuedSequence { get; set; }
        public long SizeInBytes { get; set; }
        public string Label { get; set; }
        public DateTime Enqueued { get; set; }
        public DateTime Expires { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; } 
        public string Body { get; set; }

        public string ContentType { get; set; }
        public int DeliveryCount { get; set; }
        public DateTime LockedUntil { get; set; }
        public string LockToken { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToSessionId { get; set; }
        public DateTime ScheduledEnqueueTime { get; set; }
        public string SessionId { get; set; }
        public TimeSpan TimeToLive { get; set; }
        public string To { get; set; }
    }
}
