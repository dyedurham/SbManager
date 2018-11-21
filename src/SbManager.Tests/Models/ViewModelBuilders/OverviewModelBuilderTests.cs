using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SbManager.BusHelpers;
using SbManager.Models.ViewModelBuilders;
using SbManager.Models.ViewModels;
using Shouldly;
using TestStack.BDDfy;

namespace SbManager.Tests.Models.ViewModelBuilders
{
    [TestFixture]
    public class OverviewModelBuilderTests
    {
        private IBusMonitor _busMonitor;
        private Overview _result;
        private OverviewModelBuilder _builder;
        private Overview _expected;

        [Test]
        public void CanGetOverview()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview())
                .When(x => x.WhenBuildingModel())
                .Then(x => x.ThenTheOverviewIsReturned())
                .BDDfy();
        }

        void GivenABuilder()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            _builder = new OverviewModelBuilder(_busMonitor);
        }

        void GivenThatTheBusMonitorReturnsAnOverview()
        {
            _expected = new Overview {Queues = new List<Queue>()};
            _busMonitor.GetOverview().Returns(_expected);
        }

        async Task WhenBuildingModel()
        {
            _result = await _builder.Build();
        }

        void ThenTheOverviewIsReturned()
        {
            _result.ShouldBe(_expected);
        }
    }
}
