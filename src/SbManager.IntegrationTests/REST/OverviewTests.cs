using System.Net;
using NUnit.Framework;
using TestStack.BDDfy;

namespace SbManager.IntegrationTests.REST
{
    [TestFixture(Category = Constants.IntegrationTests)]
    public class OverviewTests : RestTestBase
    {
        [Test]
        public void CanLoadOverview()
        {
            this.Given(x => x.GivenARestClient())
                .And(x => x.GivenAGetRequest("/api/v1/busmanager"))
                .When(x => WhenPerformingGetRequest())
                .Then(x => x.ThenShouldHaveHttpStatus(HttpStatusCode.OK))
                .BDDfy();
        }

    }
}
