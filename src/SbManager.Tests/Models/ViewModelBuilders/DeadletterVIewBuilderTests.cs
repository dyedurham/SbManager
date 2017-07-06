using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DeadletterVIewBuilderTests
    {
        private IBusMonitor _busMonitor;
        public string FakeUrl = "http://url";
        private DeadletterView _result;
        private DeadletterViewBuilder _builder;
        [Test]
        public void CanListDeadLettersFromSubscriptionsAndQueues()
        {
            this.Given(x => x.GivenABuilder())
                .And(x => x.GivenThatTheBusMonitorReturnsAnOverview())
                .When(x => x.WhenBuildingModel())
                .Then(x => x.ThenDeadlettersAreReturned())
                .BDDfy();
        }

        void GivenABuilder()
        {
            _busMonitor = Substitute.For<IBusMonitor>();
            var config = Substitute.For<IConfig>();
            config.WebAppUrl.Returns(FakeUrl);
            _builder = new DeadletterViewBuilder(_busMonitor, config);
        }


        void GivenThatTheBusMonitorReturnsAnOverview()
        {
            var overview = new Overview {
                Queues = new List<Queue>
                {
                    new Queue {DeadLetterCount = 0, Name = "good.queue"},
                    new Queue {DeadLetterCount = 1, Name = "bad.queue"},
                    new Queue {DeadLetterCount = 10000, Name = "ohmahgerd.queue"}
                },
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        Subscriptions = new List<Subscription>
                        {
                            new Subscription {DeadLetterCount = 0, Name = "good.sub.hi", TopicName = "hi"}
                        }
                    },
                    new Topic
                    {
                        Subscriptions = new List<Subscription>()
                    },
                    new Topic
                    {
                        Subscriptions = new List<Subscription>
                        {
                            new Subscription {DeadLetterCount = 0, Name = "good.sub.bye", TopicName = "bye"},
                            new Subscription {DeadLetterCount = 3, Name = "bad.sub.bye", TopicName = "bye"}
                        }
                    }
                }
            };
            _busMonitor.GetOverview(Arg.Any<Boolean>()).Returns(overview);
        }

        void WhenBuildingModel()
        {
            _result = _builder.Build();
        }

        void ThenDeadlettersAreReturned()
        {
            _result.Deadletters.Count.ShouldBe(3);

            _result.Deadletters.ShouldContain(dl => dl.Name == "bad.queue" && dl.DeadLetterCount == 1 && dl.Href == $"{FakeUrl}/#/queue/bad.queue");

            _result.Deadletters.ShouldContain(dl => dl.Name == "ohmahgerd.queue" && dl.DeadLetterCount == 10000 && dl.Href == $"{FakeUrl}/#/queue/ohmahgerd.queue");

            _result.Deadletters.ShouldContain(dl => dl.Name == "bad.sub.bye" && dl.DeadLetterCount == 3 && dl.Href == $"{FakeUrl}/#/topic/bye/bad.sub.bye");
        }
    }
}
