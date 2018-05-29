using System.Linq;
using System.Threading.Tasks;
using Mossharbor.AzureWorkArounds.ServiceBus;
using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteAllDeadLettersCommand : ICommand
    {
    }

    public class DeleteAllDeadLettersCommandHandler : ICommandHandler<DeleteAllDeadLettersCommand>
    {
        private readonly NamespaceManager _namespaceManager;
        private readonly IRequeueAndRemove _requeueAndRemove;
        private const string DeadLetterQueueSuffix = "/$DeadLetterQueue";
        public DeleteAllDeadLettersCommandHandler(NamespaceManager namespaceManager, IRequeueAndRemove requeueAndRemove)
        {
            _namespaceManager = namespaceManager;
            _requeueAndRemove = requeueAndRemove;
        }

        public async Task Execute(DeleteAllDeadLettersCommand command)
        {
            var topics = _namespaceManager.GetTopics().ToArray();
            foreach (var topic in topics)
            {
                var subscriptions = _namespaceManager.GetSubscriptions(topic.Path);
                foreach (var subscription in subscriptions)
                {
                    await _requeueAndRemove.RemoveAll(topic.Path, subscription.Name + DeadLetterQueueSuffix);
                }
            }

            var queues = _namespaceManager.GetQueues().ToArray();
            foreach (var queue in queues)
            {
                await _requeueAndRemove.RemoveAll(queue.Path + DeadLetterQueueSuffix);
            }
        }
    }
}