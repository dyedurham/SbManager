using System.Threading.Tasks;

namespace SbManager.CQRS.Commands
{
    public interface ICommandSender
    {
        Task Send<TCommand>(TCommand command) where TCommand : class, ICommand;
        Task<TResult> SendWithResult<TCommand, TResult>(TCommand command) where TCommand : class, ICommand;
    }

    public interface ICommandHandlerBase { }
    public interface ICommand { }

    public interface ICommandHandler<in TCommand> : ICommandHandlerBase
        where TCommand : class, ICommand
    {
        Task Execute(TCommand command);
    }

    public interface ICommandHandlerWithResult<in TCommand, TResult> : ICommandHandler<TCommand>, ICommandHandlerBase
        where TCommand : class, ICommand
    {
        Task<TResult> ExecuteWithResult(TCommand command);
    }
}