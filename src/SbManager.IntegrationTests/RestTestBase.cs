using System.Net;
using RestSharp;
using Shouldly;

namespace SbManager.IntegrationTests
{
    public abstract class RestTestBase
    {
        protected RestClient Rest;
        protected RestRequest Request;
        protected IRestResponse Response;

        protected void GivenARestClient()
        {
            Rest = new RestClient("http://localhost:8062") { FollowRedirects = false };
        }

        protected void GivenAGetRequest(string path)
        {
            Request = new RestRequest(path);
        }

        protected void WhenPerformingGetRequest()
        {
            Response = Rest.Get(Request);
        }

        protected void ThenShouldHaveHttpStatus(HttpStatusCode code)
        {
            Response.StatusCode.ShouldBe(code);
        }
    }
}
