using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using SbManager.BusHelpers;
using SbManager.InternalCommandHandlers;
using SbManager.Models.ViewModels;
using TestStack.BDDfy;

namespace SbManager.Tests.InternalCommandHandlers
{
    [TestFixture]
    public class RefreshCachedOverviewCommandHandlerTests
    {
        private IBusMonitor _busMonitor;
        private RefreshCachedOverviewCommandHandler _handler;
        private Overview _expected;
        private RefreshCachedOverviewCommand _command;

        [Test]
        public void CanHandleCommand()
        {
            this.Given(x => x.GivenAHandler())
                .And(x => x.GivenACommand())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview())
                .When(x => x.WhenExecutingCommand())
                .Then(x => x.ThenTheBusMonitorIsCalledWithForceFlag())
                .BDDfy();
        }

        void GivenAHandler()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            _handler = new RefreshCachedOverviewCommandHandler(_busMonitor);
        }

        void GivenACommand()
        {
            _command = new RefreshCachedOverviewCommand();
        }

        void GivenThatTheBusMonitorReturnsAnOverview()
        {
            _expected = new Overview { Queues = new List<Queue>() };
            _busMonitor.GetOverview().Returns(_expected);
        }

        void WhenExecutingCommand()
        {
            _handler.Execute(_command);
        }

        void ThenTheBusMonitorIsCalledWithForceFlag()
        {
            _busMonitor.Received().GetOverview(true);
        }
    }
}
