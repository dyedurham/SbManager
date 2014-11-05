using Microsoft.ServiceBus;
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
        private readonly NamespaceManager _namespaceManager;

        public DeleteQueueCommandHandler(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager;
        }

        public void Execute(DeleteQueueCommand command)
        {
            _namespaceManager.DeleteQueue(command.Path);
        }
    }
}
