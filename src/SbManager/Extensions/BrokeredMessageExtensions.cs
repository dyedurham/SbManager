using System.Text;
using Microsoft.Azure.ServiceBus;

namespace SbManager.Extensions
{
    public static class BrokeredMessageExtensions
    {
        public static void RemoveProperties(this Message message, params string[] propertynames)
        {
            foreach (var propertyname in propertynames)
            {
                if (message.UserProperties.ContainsKey(propertyname)) message.UserProperties.Remove(propertyname);
            }
        }

        public static string GetBodyString(this Message message)
        {
            return message.Body != null ? Encoding.UTF8.GetString(message.Body) : null;
        }
    }
}
