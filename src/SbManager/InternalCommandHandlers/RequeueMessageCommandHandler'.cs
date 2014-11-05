using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class RequeueMessageCommand : ICommand
    {
        public RequeueMessageCommand(string messageId, string queue)
        {
            MessageId = messageId;
            Queue = queue;
        }

        public RequeueMessageCommand(string messageId, string topicName, string subscription)
        {
            MessageId = messageId;
            TopicName = topicName;
            Subscription = subscription;
        }

        public string MessageId { get; set; }
        public string Queue { get; set; }
        public string TopicName { get; set; }
        public string Subscription { get; set; }
        public string NewBody { get; set; }
    }
    public class RequeueMessageCommandHandler : CQRS.Commands.ICommandHandler<RequeueMessageCommand>
    {
        private readonly IRequeueAndRemove _requeueAndRemove;

        public RequeueMessageCommandHandler(IRequeueAndRemove requeueAndRemove)
        {
            _requeueAndRemove = requeueAndRemove;
        }

        public void Execute(RequeueMessageCommand command)
        {
            if (command.Queue != null)
                _requeueAndRemove.RequeueOne(command.Queue, command.MessageId, command.NewBody);
            else
                _requeueAndRemove.RequeueOne(command.TopicName, command.Subscription, command.MessageId, command.NewBody);
        }
    }
}
