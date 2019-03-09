using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeleteAllDeadLettersCommand : ICommand
    {
    }

    public class DeleteAllDeadLettersCommandHandler : ICommandHandler<DeleteAllDeadLettersCommand>
    {
        private readonly ManagementClient _managementClient;
        private readonly IRequeueAndRemove _requeueAndRemove;
        public DeleteAllDeadLettersCommandHandler(ManagementClient managementClient, IRequeueAndRemove requeueAndRemove)
        {
            _managementClient = managementClient;
            _requeueAndRemove = requeueAndRemove;
        }

        public async Task Execute(DeleteAllDeadLettersCommand command)
        {
            var topics = await _managementClient.GetTopicsAsync();
            foreach (var topic in topics)
            {
                var subscriptions = await _managementClient.GetSubscriptionsAsync(topic.Path);
                foreach (var subscription in subscriptions)
                {
                    await _requeueAndRemove.RemoveAll(topic.Path, EntityNameHelper.FormatDeadLetterPath(subscription.SubscriptionName));
                }
            }

            var queues = await _managementClient.GetQueuesAsync();
            foreach (var queue in queues)
            {
                await _requeueAndRemove.RemoveAll(EntityNameHelper.FormatDeadLetterPath(queue.Path));
            }
        }
    }
}