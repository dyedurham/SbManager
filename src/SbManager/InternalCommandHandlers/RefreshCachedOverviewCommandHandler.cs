using SbManager.BusHelpers;
using SbManager.CQRS.Commands;

namespace SbManager.InternalCommandHandlers
{
    public class RefreshCachedOverviewCommand : ICommand { }

    public class RefreshCachedOverviewCommandHandler : CQRS.Commands.ICommandHandler<RefreshCachedOverviewCommand>
    {
        private readonly IBusMonitor _busMonitor;

        public RefreshCachedOverviewCommandHandler(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }
        public void Execute(RefreshCachedOverviewCommand command)
        {
            _busMonitor.GetOverview(true);
        }
    }
}
