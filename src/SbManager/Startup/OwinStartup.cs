using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Nancy.Bootstrapper;
using Owin;

namespace SbManager.Startup
{
    public interface IOwinStartup
    {
        void Configuration(IAppBuilder app);
    }
    public class OwinStartup : IOwinStartup
    {
        private readonly INancyBootstrapper _nancyBootstrapper;

        public OwinStartup(INancyBootstrapper nancyBootstrapper)
        {
            _nancyBootstrapper = nancyBootstrapper;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            
            //For serving up static files
            foreach (var staticDirectory in Constants.StaticDirectories)
            {
                app.UseStaticFiles(staticDirectory);
            }

            app.Use(SupportReponseTypeByContentType); 
            app.UseNancy(a => { a.Bootstrapper = _nancyBootstrapper; });
        }

        public static Task SupportReponseTypeByContentType(IOwinContext context, Func<Task> next)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Accept) || context.Request.Accept == "*/*")
            {
                if (context.Request.Headers.ContainsKey("Content-Type"))
                {
                    if (context.Request.Headers["Content-Type"] == "application/json")
                        context.Request.Accept = "application/json";
                    if (context.Request.Headers["Content-Type"] == "text/xml")
                        context.Request.Accept = "text/xml";
                }
            }
            return next.Invoke();
        }
    }
}
