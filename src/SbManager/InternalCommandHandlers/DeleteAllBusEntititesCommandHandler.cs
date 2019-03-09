using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteAllBusEntititesCommand : ICommand { }

    public class DeleteAllBusEntititesCommandHandler : CQRS.Commands.ICommandHandler<DeleteAllBusEntititesCommand>
    {
        private readonly ManagementClient _managementClient;

        public DeleteAllBusEntititesCommandHandler(ManagementClient managementClient)
        {
            _managementClient = managementClient;
        }

        public async Task Execute(DeleteAllBusEntititesCommand command)
        {
            var deleteQueuesTasks = (await _managementClient.GetQueuesAsync()).Select(e => _managementClient.DeleteQueueAsync(e.Path));
            var deleteTopicsTasks = (await _managementClient.GetTopicsAsync()).Select(e => _managementClient.DeleteTopicAsync(e.Path));
            await Task.WhenAll(deleteQueuesTasks.Concat(deleteTopicsTasks));
        }
    }
}
