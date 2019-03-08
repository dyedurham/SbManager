using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteQueueCommand : ICommand
    {
        public DeleteQueueCommand(string path)
        {
            Path = path;
        }
        public string Path { get; set; }
    }

    public class DeleteQueueCommandHandler : CQRS.Commands.ICommandHandler<DeleteQueueCommand>
    {
        private readonly ManagementClient _managementClient;

        public DeleteQueueCommandHandler(ManagementClient managementClient)
        {
            _managementClient = managementClient;
        }

        public async Task Execute(DeleteQueueCommand command)
        {
            await _managementClient.DeleteQueueAsync(command.Path);
        }
    }
}
