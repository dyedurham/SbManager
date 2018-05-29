using System.Threading.Tasks;
using Mossharbor.AzureWorkArounds.ServiceBus;
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
        private readonly NamespaceManager _namespaceManager;

        public DeleteSubscriptionCommandHandler(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager;
        }

        public Task Execute(DeleteSubscriptionCommand command)
        {
            _namespaceManager.DeleteSubscription(command.Topic, command.Path);
            return Task.CompletedTask;
        }
    }
}
