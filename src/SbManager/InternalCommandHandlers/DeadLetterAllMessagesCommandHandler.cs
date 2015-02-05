using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class DeadLetterAllMessagesCommand : ICommand
    {
        public DeadLetterAllMessagesCommand(string queue)
        {
            Queue = queue;
        }

        public DeadLetterAllMessagesCommand(string topicName, string subscription)
        {
            TopicName = topicName;
            Subscription = subscription;
        }

        public string Queue { get; set; }
        public string TopicName { get; set; }
        public string Subscription { get; set; }
    }
    public class DeadLetterAllMessagesCommandHandler : ICommandHandler<DeadLetterAllMessagesCommand>
    {
        private readonly IRequeueAndRemove _requeueAndRemove;

        public DeadLetterAllMessagesCommandHandler(IRequeueAndRemove requeueAndRemove)
        {
            _requeueAndRemove = requeueAndRemove;
        }

        public void Execute(DeadLetterAllMessagesCommand command)
        {
            if (command.Queue != null)
                _requeueAndRemove.KillAll(command.Queue);
            else
                _requeueAndRemove.KillAll(command.TopicName, command.Subscription);
        }
    }
}
