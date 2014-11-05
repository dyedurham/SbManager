using Nancy;

namespace SbManager.HttpModules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
            : base("/")
        {
            Get["/"] = _ => View["index", new
            {
                Title = string.Format("{0} - Home", Constants.AppName),
                Welcome = string.Format("Welcome to the {0} Home page", Constants.AppName)
            }];
        }
    }
}

