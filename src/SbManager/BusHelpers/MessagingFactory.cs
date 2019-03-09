using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace SbManager.BusHelpers
{
    public class MessagingFactory
    {
        public static MessagingFactory CreateFromConnectionString(string connectionString)
        {
            return new MessagingFactory(connectionString);
        }

        private readonly ServiceBusConnection _connection;

        private MessagingFactory(string connectionString)
        {
            _connection = new ServiceBusConnection(connectionString);
        }

        public MessageReceiver CreateMessageReceiver(string path)
        {
            return new MessageReceiver(_connection, path);
        }

        public MessageSender CreateMessageSender(string path)
        {
            return new MessageSender(_connection, path);
        }
    }
}