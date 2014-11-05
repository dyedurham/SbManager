using System;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace SbManager.Extensions
{
    public static class BrokeredMessageExtensions
    {
        public static void RemoveProperties(this BrokeredMessage message, params string[] propertynames)
        {
            foreach (var propertyname in propertynames)
            {
                if (message.Properties.ContainsKey(propertyname)) message.Properties.Remove(propertyname);
            }
        }

        public static string GetBodyString(this BrokeredMessage message)
        {
            try
            {
                var stream = message.GetBody<Stream>();
                if (stream == null) return null;
                return new StreamReader(stream).ReadToEnd();
            }
            catch (InvalidOperationException lockException)
            {
                if (!lockException.Message.Contains("Operation is not valid due to the current state of the object")) throw;
                return null;
            }
        }
    }
}
