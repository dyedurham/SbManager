using NUnit.Framework;
using SbManager.Extensions;
using Shouldly;
using TestStack.BDDfy;

namespace SbManager.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private string _str;
        private string _result;
        private bool _isDeadLetter;

        [Test]
        public void CanUnescapePathName()
        {
            this.Given(x => x.GivenAString("a_b_c"))
                .When(x => x.WhenUnescapingPathName())
                .Then(x => x.ThenTheResultShouldEqual("a/b/c"))
                .BDDfy();
        }
        [Test]
        public void CanRemoveDeadLetterPath()
        {
            this.Given(x => x.GivenAString("abc/$DeadLetterQueue"))
                .When(x => x.WhenRemovingDeadLetterPath())
                .Then(x => x.ThenTheResultShouldEqual("abc"))
                .BDDfy();
        }
        [Test]
        public void CanSurviveRemoveDeadLetterPathWhenNotDeadLetter()
        {
            this.Given(x => x.GivenAString("abc/def"))
                .When(x => x.WhenRemovingDeadLetterPath())
                .Then(x => x.ThenTheResultShouldEqual("abc/def"))
                .BDDfy();
        }

        [Test]
        public void CanMakeDeadLetterPath()
        {
            this.Given(x => x.GivenAString("abc"))
                .When(x => x.WhenMakingDeadLetterPath())
                .Then(x => x.ThenTheResultShouldEqual("abc/$DeadLetterQueue"))
                .BDDfy();
        }

        [Test]
        public void CanCorrectlyMakeDeadLetterPathWhenAlreadyDeadLetterPath()
        {
            this.Given(x => x.GivenAString("abc/$DeadLetterQueue"))
                .When(x => x.WhenMakingDeadLetterPath())
                .Then(x => x.ThenTheResultShouldEqual("abc/$DeadLetterQueue"))
                .BDDfy();
        }

        [Test]
        public void CanDetectDeadLetterPath()
        {
            this.Given(x => x.GivenAString("abc/$DeadLetterQueue"))
                .When(x => x.WhenCheckingIsDeadLetterPath())
                .Then(x => x.ThenTheFlagShouldBe(true))
                .BDDfy();
        }

        [Test]
        public void CanDetectNonDeadLetterPath()
        {
            this.Given(x => x.GivenAString("abc"))
                .When(x => x.WhenCheckingIsDeadLetterPath())
                .Then(x => x.ThenTheFlagShouldBe(false))
                .BDDfy();
        }

        void GivenAString(string str)
        {
            _str = str;
        }

        void WhenUnescapingPathName()
        {
            _result = _str.UnescapePathName();
        }

        void WhenRemovingDeadLetterPath()
        {
            _result = _str.RemoveDeadLetterPath();
        }
        void WhenMakingDeadLetterPath()
        {
            _result = _str.MakeDeadLetterPath();
        }

        void WhenCheckingIsDeadLetterPath()
        {
            _isDeadLetter = _str.IsDeadLetterPath();
        }

        void ThenTheResultShouldEqual(string res)
        {
            _result.ShouldBe(res);
        }

        void ThenTheFlagShouldBe(bool flag)
        {
            _isDeadLetter.ShouldBe(flag);
        }
    }
}
