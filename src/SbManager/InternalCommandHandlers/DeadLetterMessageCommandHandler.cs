using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeadLetterMessageCommand : ICommand
    {
        public DeadLetterMessageCommand(string messageId, string queue)
        {
            MessageId = messageId;
            Queue = queue;
        }

        public DeadLetterMessageCommand(string messageId, string topicName, string subscription)
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
    public class DeadLetterMessageCommandHandler : CQRS.Commands.ICommandHandler<DeadLetterMessageCommand>
    {
        private readonly IRequeueAndRemove _requeueAndRemove;

        public DeadLetterMessageCommandHandler(IRequeueAndRemove requeueAndRemove)
        {
            _requeueAndRemove = requeueAndRemove;
        }

        public void Execute(DeadLetterMessageCommand command)
        {
            if (command.Queue != null)
                _requeueAndRemove.Kill(command.Queue, command.MessageId);
            else
                _requeueAndRemove.Kill(command.TopicName, command.Subscription, command.MessageId);
        }
    }
}
