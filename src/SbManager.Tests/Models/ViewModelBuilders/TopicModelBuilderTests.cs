using System;
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
    public class TopicModelBuilderTests
    {
        private IBusMonitor _busMonitor;
        private Topic _result;
        private TopicModelBuilder _builder;
        private Topic _expected;
        private Exception _ex;

        [Test]
        public void CanGetTopic()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testtopic"))
                .Then(x => x.ThenTheTopicIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetTopicWithCaseInsensitiveMatch()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testToPiC"))
                .Then(x => x.ThenTheTopicIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetTopicWithForceFresh()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(true))
                .When(x => x.WhenBuildingModel("testtopic"))
                .Then(x => x.ThenTheTopicIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void WillFailOnDuplicateTopics()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateTopics())
                .When(x => x.WhenBuildingModel("testtopic"))
                .Then(x => x.ThenThereShouldBeAnException())
                .BDDfy();
        }

        void GivenABuilder()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            _builder = new TopicModelBuilder(_busMonitor);
        }

        void GivenThatTheBusMonitorReturnsAnOverview(bool expectFresh)
        {
            _expected = new Topic { Name = "testtopic" };
            var overview = new Overview
            {
                Topics = new List<Topic>
            {
                _expected,
                new Topic { Name="testtopic2" }
            }
            };
            _busMonitor.GetOverview(expectFresh).Returns(overview);
        }
        void GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateTopics()
        {
            _expected = new Topic { Name = "testtopic" };
            var overview = new Overview
            {
                Topics = new List<Topic>
            {
                _expected,
                new Topic { Name="testtopic" }
            }
            };
            _busMonitor.GetOverview().Returns(overview);
        }

        async Task WhenBuildingModel(string topicname)
        {
            try
            {
                _result = await _builder.Build(new TopicCriteria(topicname));
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
        }

        void ThenTheTopicIsReturned()
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
