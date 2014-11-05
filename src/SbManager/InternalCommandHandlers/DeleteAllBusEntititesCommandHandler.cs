using System.Linq;
using Microsoft.ServiceBus;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteAllBusEntititesCommand : ICommand { }

    public class DeleteAllBusEntititesCommandHandler : CQRS.Commands.ICommandHandler<DeleteAllBusEntititesCommand>
    {
        private readonly NamespaceManager _namespaceManager;

        public DeleteAllBusEntititesCommandHandler(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager;
        }

        public void Execute(DeleteAllBusEntititesCommand command)
        {
            foreach(var q in _namespaceManager.GetQueues().ToArray()) _namespaceManager.DeleteQueue(q.Path);
            foreach(var t in _namespaceManager.GetTopics().ToArray()) _namespaceManager.DeleteTopic(t.Path);
        }
    }
}
