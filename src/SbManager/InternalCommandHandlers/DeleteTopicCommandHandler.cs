using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteTopicCommand : ICommand
    {
        public DeleteTopicCommand(string path)
        {
            Path = path;
        }
        public string Path { get; set; }
    }

    public class DeleteTopicCommandHandler : CQRS.Commands.ICommandHandler<DeleteTopicCommand>
    {
        private readonly ManagementClient _managementClient;

        public DeleteTopicCommandHandler(ManagementClient managementClient)
        {
            _managementClient = managementClient;
        }

        public async Task Execute(DeleteTopicCommand command)
        {
            await _managementClient.DeleteTopicAsync(command.Path);
        }
    }
}
