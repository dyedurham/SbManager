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
    public class SubscriptionModelBuilderTests
    {
        private IBusMonitor _busMonitor;
        private Subscription _result;
        private SubscriptionModelBuilder _builder;
        private Subscription _expected;
        private Exception _ex;

        [Test]
        public void CanGetSubscription()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testtopic", "testsubscription", false))
                .Then(x => x.ThenTheSubscriptionIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetSubscriptionWithCaseInsensitiveMatch()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(false))
                .When(x => x.WhenBuildingModel("testtopic", "testSuBscription", false))
                .Then(x => x.ThenTheSubscriptionIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void CanGetSubscriptionWithForceFresh()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview(true))
                .When(x => x.WhenBuildingModel("testtopic", "testsubscription", true))
                .Then(x => x.ThenTheSubscriptionIsReturned())
                .Then(x => x.ThenThereShouldBeNoException())
                .BDDfy();
        }

        [Test]
        public void WillFailOnDuplicateSubscriptions()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateSubscriptions())
                .When(x => x.WhenBuildingModel("testtopic", "testsubscription", false))
                .Then(x => x.ThenThereShouldBeAnException())
                .BDDfy();
        }

        void GivenABuilder()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            _builder = new SubscriptionModelBuilder(_busMonitor);
        }

        void GivenThatTheBusMonitorReturnsAnOverview(bool expectFresh)
        {
            _expected = new Subscription { Name = "testsubscription" };
            var overview = new Overview
            {
                Topics = new List<Topic>
                {
                    new Topic { Name="testtopic", Subscriptions = new List<Subscription> { _expected}},
                    new Topic { Name="testtopic2", Subscriptions = new List<Subscription> { new Subscription { Name="testsubscription2" }}},
                }
            };
            _busMonitor.GetOverview(expectFresh).Returns(overview);
        }
        void GivenThatTheBusMonitorReturnsAnOverviewWithDuplicateSubscriptions()
        {
            _expected = new Subscription { Name = "testsubscription" };
            var overview = new Overview
            {
                Topics = new List<Topic>
                {
                    new Topic { Name="testtopic", Subscriptions = new List<Subscription> { _expected, _expected}},
                    new Topic { Name="testtopic2", Subscriptions = new List<Subscription> { _expected}},
                }
            };
            _busMonitor.GetOverview().Returns(overview);
        }

        void WhenBuildingModel(string topicname, string subscriptionname, bool forceFresh)
        {
            try
            {
                _result = _builder.Build(new SubscriptionCriteria(topicname, subscriptionname, forceFresh));
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
        }

        void ThenTheSubscriptionIsReturned()
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
