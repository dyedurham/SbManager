using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Newtonsoft.Json;
using Shouldly;

namespace SbManager.Tests.BaseTests
{
    public abstract class BaseBrowserTest
    {
        protected Browser Browser;
        protected BrowserResponse Response;

        public void WhenPostingJson(string path, object jsonObject)
        {
            Response = Browser.Post(path, with =>
            {
                with.JsonBody(jsonObject);
                with.Accept(new MediaRange("application/json"));
            });
        }

        public void WhenGettingJson(string path)
        {
            Response = Browser.Get(path, with => with.Accept(new MediaRange("application/json")));
        }

        public void ThenShouldHaveResponseCode(HttpStatusCode code)
        {
            Response.StatusCode.ShouldBe(code);
        }

        public T GetResponse<T>()
        {
            return JsonConvert.DeserializeObject<T>(Response.Body.AsString());
        }
    }
}
