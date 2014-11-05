using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nancy;
using Nancy.Testing;
using NSubstitute;
using NUnit.Framework;
using SbManager.BusHelpers;
using SbManager.CQRS.Commands;
using SbManager.CQRS.ModelBuilders;
using SbManager.HttpModules.V1;
using SbManager.InternalCommandHandlers;
using SbManager.Models.ViewModelBuilders;
using SbManager.Models.ViewModels;
using SbManager.Tests.BaseTests;
using Shouldly;
using TestStack.BDDfy;

namespace SbManager.Tests.Modules.V1
{
    [TestFixture]
    public class BusManagerModuleTests : BaseBrowserTest
    {
        private IRequeueAndRemove _requeueAndRemove;
        private IModelCreator _modelCreator;
        private ICommandSender _commandSender;

        [Test]
        public void CanGetOverview()
        {
            this
                .Given(x => GivenABrowser())
                .And(x => GivenAMockedOverviewBuilder())
                .When(x => WhenGettingJson("/api/v1/busmanager"))
                .Then(x => ThenShouldHaveResponseCode(HttpStatusCode.OK))
                .And(x => ThenThereIsOverviewDataReturned())
                .BDDfy();
        }

        [Test]
        public void CanDeleteAll()
        {
            this
                .Given(x => GivenABrowser())
                .When(x => WhenPostingJson("/api/v1/busmanager/deleteall", null))
                .Then(x => ThenCommandWasSent<DeleteAllBusEntititesCommand>())
                .Then(x => ThenShouldHaveResponseCode(HttpStatusCode.OK))
                .And(x => ThenReturnedJsonHasSuccessFlag())
                .BDDfy();
        }

        [Test]
        public void CanGetTopic()
        {
            this
                .Given(x => GivenABrowser())
                .And(x => GivenAMockedOverviewBuilder())
                .And(x => GivenAMockedTopicBuilder("testtopic"))
                .When(x => WhenGettingJson("/api/v1/busmanager/topic/1"))
                .Then(x => ThenBuilderWasCalledWithCriteria<Topic, TopicCriteria>(c => c.Topic == "1"))
                .Then(x => ThenShouldHaveResponseCode(HttpStatusCode.OK))
                .And(x => ThenTheMessagingEntityIsReturned("testtopic"))
                .BDDfy();
        }

        [Test]
        public void CanGetSubscription()
        {
            this
                .Given(x => GivenABrowser())
                .And(x => GivenAMockedOverviewBuilder())
                .And(x => GivenAMockedSubscriptionBuilder("testsub"))
                .When(x => WhenGettingJson("/api/v1/busmanager/topic/2/3"))
                .Then(x => ThenBuilderWasCalledWithCriteria<Subscription, SubscriptionCriteria>(c => c.Topic == "2" && c.Subscription == "3"))
                .Then(x => ThenShouldHaveResponseCode(HttpStatusCode.OK))
                .And(x => ThenTheMessagingEntityIsReturned("testsub"))
                .BDDfy();
        }

        [Test]
        public void CanGetQueue()
        {
            this
                .Given(x => GivenABrowser())
                .And(x => GivenAMockedOverviewBuilder())
                .And(x => GivenAMockedQueueBuilder("testqueue"))
                .When(x => WhenGettingJson("/api/v1/busmanager/queue/4"))
                .Then(x => ThenBuilderWasCalledWithCriteria<Queue, QueueCriteria>(c => c.Queue == "4"))
                .Then(x => ThenShouldHaveResponseCode(HttpStatusCode.OK))
                .And(x => ThenTheMessagingEntityIsReturned("testqueue"))
                .BDDfy();
        }

        void GivenABrowser()
        {
            _requeueAndRemove = Substitute.For<IRequeueAndRemove>();
            _modelCreator = Substitute.For<IModelCreator>();
            _commandSender = Substitute.For<ICommandSender>();

            Browser = new Browser(with =>
            {
                with.Module<BusManagerModule>();
                with.Dependency(_requeueAndRemove);
                with.Dependency(_modelCreator);
                with.Dependency(_commandSender);
            });
        }

        void GivenAMockedOverviewBuilder()
        {
            _modelCreator.Build<Overview>().Returns(new Overview()
            {
                Queues = new List<Queue>
                {
                    new Queue { Name = "testqueue" }
                }
            });
        }

        void GivenAMockedTopicBuilder(string entityName)
        {
            _modelCreator.Build<Topic, TopicCriteria>(Arg.Any<TopicCriteria>()).Returns(new Topic()
            {
                Name = entityName
            });
        }

        void GivenAMockedQueueBuilder(string entityName)
        {
            _modelCreator.Build<Queue, QueueCriteria>(Arg.Any<QueueCriteria>()).Returns(new Queue()
            {
                Name = entityName
            });
        }

        void GivenAMockedSubscriptionBuilder(string entityName)
        {
            _modelCreator.Build<Subscription, SubscriptionCriteria>(Arg.Any<SubscriptionCriteria>()).Returns(new Subscription()
            {
                Name = entityName
            });
        }

        void ThenThereIsOverviewDataReturned()
        {
            var returned = GetResponse<Overview>();
            returned.ShouldNotBe(null);
            returned.Queues.ShouldNotBe(null);
            returned.Queues.First().Name.ShouldBe("testqueue");
        }

        void ThenReturnedJsonHasSuccessFlag()
        {
            var returned = GetResponse<dynamic>();
            ShouldBeTestExtensions.ShouldNotBe(returned, null);
            ShouldBeTestExtensions.ShouldBe((bool)returned.success, true);
        }

        void ThenTheMessagingEntityIsReturned(string entityName)
        {
            var returned = GetResponse<dynamic>();
            ShouldBeTestExtensions.ShouldNotBe(returned, null);
            ShouldBeTestExtensions.ShouldBe((string)returned.Name, entityName);
        }
        
        void ThenCommandWasSent<T>() where T : class, ICommand
        {
            ThenCommandWasSent<T>(c => c != null);
        }
        void ThenCommandWasSent<T>(Expression<Predicate<T>> assert) where T : class, ICommand
        {
            _commandSender.Received().Send(Arg.Is(assert));
        }
        void ThenBuilderWasCalledWithCriteria<TModelType, TCriteriaType>(Expression<Predicate<TCriteriaType>> assert) where TModelType : class where TCriteriaType : class
        {
            _modelCreator.Received().Build<TModelType, TCriteriaType>(Arg.Is(assert));
        }
    }
}
