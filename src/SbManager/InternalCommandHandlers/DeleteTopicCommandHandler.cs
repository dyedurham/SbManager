using Microsoft.ServiceBus;
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
        private readonly NamespaceManager _namespaceManager;

        public DeleteTopicCommandHandler(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager;
        }

        public void Execute(DeleteTopicCommand command)
        {
            _namespaceManager.DeleteTopic(command.Path);
        }
    }
}
