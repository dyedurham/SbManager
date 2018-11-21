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

        void ThenTheResultShouldEqual(string res)
        {
            _result.ShouldBe(res);
        }
    }
}
