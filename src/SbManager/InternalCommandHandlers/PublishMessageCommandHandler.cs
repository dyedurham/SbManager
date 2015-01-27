using SbManager.BusHelpers;
using SbManager.CQRS.Commands;
using SbManager.Models.ViewModels;

namespace SbManager.InternalCommandHandlers
{
    public class PublishMessageCommand : ICommand
    {
        public PublishMessageCommand(string topic, Message message)
        {
            Topic = topic;
            Message = message;
        }
        public string Topic { get; set; }
        public Message Message { get; set; }
    }

    public class PublishMessageCommandHandler : ICommandHandler<PublishMessageCommand>
    {
        private readonly ISender _sender;

        public PublishMessageCommandHandler(ISender sender)
        {
            _sender = sender;
        }

        public void Execute(PublishMessageCommand command)
        {
            _sender.Publish(command.Message, command.Topic);
        }
    }
}
