using System;
using System.Collections.Generic;
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
    public class QueueModelBuilderTests
    {
        private IBusMonitor _busMonitor;
        private Queue _result;
        private QueueModelBuilder _builder;
        private Queue _expected;
        private Exception _ex;

        [Test]
        public void CanGetQueue()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testqueue", false))
                .Then(x => x.ThenTheQueueIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetQueueWithCaseInsensitiveMatch()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testQUeuE", false))
                .Then(x => x.ThenTheQueueIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetQueueWithForceFresh()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(true))
                .When(x => x.WhenBuildingModel("testqueue", true))
                .Then(x => x.ThenTheQueueIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void WillFailOnDuplicateQueues()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateQueues())
                .When(x => x.WhenBuildingModel("testqueue", true))
                .Then(x => x.ThenThereShouldBeAnException())
                .BDDfy();
        }

        void GivenABuilder()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            _builder = new QueueModelBuilder(_busMonitor);
        }

        void GivenThatTheBusMonitorReturnsAnOverview(bool expectFresh)
        {
            _expected = new Queue {Name = "testqueue"};
            var overview = new Overview { Queues = new List<Queue>
            {
                _expected,
                new Queue { Name="testqueue2" }
            }};
            _busMonitor.GetOverview(expectFresh).Returns(overview);
        }
        void GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateQueues()
        {
            _expected = new Queue {Name = "testqueue"};
            var overview = new Overview { Queues = new List<Queue>
            {
                _expected,
                new Queue { Name="testqueue" }
            }};
            _busMonitor.GetOverview().Returns(overview);
        }

        void WhenBuildingModel(string queuename, bool forceFresh)
        {
            try
            {
                _result = _builder.Build(new QueueCriteria(queuename, forceFresh));
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
        }

        void ThenTheQueueIsReturned()
        {
            _result.ShouldBe(_expected);
        }

        void ThenThereShouldBeNoException()
        {
            _ex.ShouldBe(null);
        }

        void ThenThereShouldBeAnException()
        {
            _ex.ShouldNotBe(null);
        }
    }
}
