using SbManager.BusHelpers;
using SbManager.CQRS.Commands;
using SbManager.Models.ViewModels;

namespace SbManager.InternalCommandHandlers
{
    public class SendMessageCommand : ICommand
    {
        public SendMessageCommand(string queue, Message message)
        {
            Queue = queue;
            Message = message;
        }
        public string Queue { get; set; }
        public Message Message { get; set; }
    }

    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly ISender _sender;

        public SendMessageCommandHandler(ISender sender)
        {
            _sender = sender;
        }

        public void Execute(SendMessageCommand command)
        {
            _sender.Send(command.Message, command.Queue);
        }
    }
}
