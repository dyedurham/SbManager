using SbManager.BusHelpers;
using SbManager.Models.ViewModels;

namespace SbManager.Models.ViewModelBuilders
{
    public class OverviewModelBuilder : CQRS.ModelBuilders.IModelBuilder<Overview>
    {
        private readonly IBusMonitor _busMonitor;

        public OverviewModelBuilder(IBusMonitor busMonitor)
        {
            _busMonitor = busMonitor;
        }
        public Overview Build()
        {
            return _busMonitor.GetOverview();
        }
    }
}
