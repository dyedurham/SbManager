using System.Threading.Tasks;
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
        public async Task<Overview> Build()
        {
            return await _busMonitor.GetOverview();
        }
    }
}
