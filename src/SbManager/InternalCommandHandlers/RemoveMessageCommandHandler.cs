using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class RemoveMessageCommand : ICommand
    {
        public RemoveMessageCommand(string messageId, string queue)
        {
            MessageId = messageId;
            Queue = queue;
        }

        public RemoveMessageCommand(string messageId, string topicName, string subscription)
        {
            MessageId = messageId;
            TopicName = topicName;
            Subscription = subscription;
        }

        public string MessageId { get; set; }
        public string Queue { get; set; }
        public string TopicName { get; set; }
        public string Subscription { get; set; }
    }
    public class RemoveMessageCommandHandler : CQRS.Commands.ICommandHandler<RemoveMessageCommand>
    {
        private readonly IRequeueAndRemove _requeueAndRemove;

        public RemoveMessageCommandHandler(IRequeueAndRemove requeueAndRemove)
        {
            _requeueAndRemove = requeueAndRemove;
        }

        public void Execute(RemoveMessageCommand command)
        {
            if (command.Queue != null)
                _requeueAndRemove.RemoveOne(command.Queue, command.MessageId);
            else
                _requeueAndRemove.RemoveOne(command.TopicName, command.Subscription, command.MessageId);
        }
    }
}
