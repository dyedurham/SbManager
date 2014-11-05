using Microsoft.ServiceBus.Messaging;
using NUnit.Framework;
using SbManager.Extensions;
using Shouldly;
using TestStack.BDDfy;

namespace SbManager.Tests.Extensions
{
    [TestFixture]
    public class BrokeredMessageExtensionsTests
    {
        private BrokeredMessage _message;
        private string _messageBody;

        [Test]
        public void CanRemoveProperties()
        {
            this.Given(x => x.GivenABrokeredMessageWithProperties("foo", "bar", "baz"))
                .When(x => x.WhenRemovingProperties("foo", "baz"))
                .Then(x => x.ThenItShouldHaveRemovedTheSpecifiedProperties("foo", "baz"))
                .Then(x => x.ThenItShouldHaveKeptTheOtherProperties("bar"))
                .BDDfy();
        }
        
        [Test]
        public void CanGetMessageBodyAsStream()
        {
            this.Given(x => x.GivenABrokeredMessageWithBody("foobar"))
                .When(x => x.WhenGettingBody())
                .Then(x => x.ThenTheBodyStringShouldContain("foobar"))
                .BDDfy();
        }
        
        [Test]
        public void CanGetNullMessageBody()
        {
            this.Given(x => x.GivenABrokeredMessageWithNoBody())
                .When(x => x.WhenGettingBody())
                .Then(x => x.ThenTheBodyStringShouldBe(null))
                .BDDfy();
        }

        void GivenABrokeredMessageWithProperties(params string[] properties)
        {
            _message = new BrokeredMessage(new TestMessage());
            foreach (var property in properties)
            {
                _message.Properties.Add(property, "test");
            }
        }
        void GivenABrokeredMessageWithBody(string testdata)
        {
            _message = new BrokeredMessage(new TestMessage { Test = testdata });
        }
        void GivenABrokeredMessageWithNoBody()
        {
            _message = new BrokeredMessage(null);
        }
        void WhenRemovingProperties(params string[] properties)
        {
            _message.RemoveProperties(properties);
        }
        void WhenGettingBody()
        {
            _messageBody = _message.GetBodyString();
        }
        void ThenItShouldHaveRemovedTheSpecifiedProperties(params string[] removed)
        {
            foreach (var rem in removed) _message.Properties.ContainsKey(rem).ShouldBe(false);
        }
        void ThenItShouldHaveKeptTheOtherProperties(params string[] remaining)
        {
            foreach (var rem in remaining) _message.Properties.ContainsKey(rem).ShouldBe(true);
        }
        void ThenTheBodyStringShouldBe(string expected)
        {
            _messageBody.ShouldBe(expected);
        }
        void ThenTheBodyStringShouldContain(string expected)
        {
            _messageBody.ShouldContain(expected);
        }
    }

    public class TestMessage
    {
        public string Test { get; set; }   
    }
}
