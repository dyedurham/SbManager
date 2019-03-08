using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteSubscriptionCommand : ICommand
    {
        public DeleteSubscriptionCommand(string topic, string path)
        {
            Topic = topic;
            Path = path;
        }
        public string Topic { get; set; }
        public string Path { get; set; }
    }

    public class DeleteSubscriptionCommandHandler : CQRS.Commands.ICommandHandler<DeleteSubscriptionCommand>
    {
        private readonly ManagementClient _managementClient;

        public DeleteSubscriptionCommandHandler(ManagementClient managementClient)
        {
            _managementClient = managementClient;
        }

        public async Task Execute(DeleteSubscriptionCommand command)
        {
            await _managementClient.DeleteSubscriptionAsync(command.Topic, command.Path);
        }
    }
}
