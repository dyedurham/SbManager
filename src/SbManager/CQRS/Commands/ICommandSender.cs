namespace SbManager.CQRS.Commands
{
    public interface ICommandSender
    {
        void Send<TCommand>(TCommand command) where TCommand : class, ICommand;
        TResult SendWithResult<TCommand, TResult>(TCommand command) where TCommand : class, ICommand;
    }

    public interface ICommandHandlerBase { }
    public interface ICommand { }

    public interface ICommandHandler<in TCommand> : ICommandHandlerBase
        where TCommand : class, ICommand
    {
        void Execute(TCommand command);
    }

    public interface ICommandHandlerWithResult<in TCommand, out TResult> : ICommandHandler<TCommand>, ICommandHandlerBase
        where TCommand : class, ICommand
    {
        TResult ExecuteWithResult(TCommand command);
    }
}