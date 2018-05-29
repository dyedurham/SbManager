using System.Threading.Tasks;
using Mossharbor.AzureWorkArounds.ServiceBus;
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

        public Task Execute(DeleteAllBusEntititesCommand command)
        {
            foreach(var q in _namespaceManager.GetQueues()) _namespaceManager.DeleteQueue(q.Path);
            foreach(var t in _namespaceManager.GetTopics()) _namespaceManager.DeleteTopic(t.Path);
            return Task.CompletedTask;
        }
    }
}
