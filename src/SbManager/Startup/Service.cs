using System;
using Autofac;
using Microsoft.Owin.Hosting;
using SbManager.CQRS;
using Serilog;

namespace SbManager.Startup
{
    public interface IService
    {
        void Start();
        void Stop();
    }
    public class Service : IService
    {
        private readonly ILogger _logger;
        private readonly IConfig _config;
        public static IContainer AppContainer;
        private IDisposable _webApp;
        private readonly IOwinStartup _owinStartup;

        public Service(IConfig config, IOwinStartup owinStartup, ILogger logger)
        {
            _logger = logger.ForContext<Service>();
            _config = config;
            _owinStartup = owinStartup;
        }

        public static void Setup()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AutofacRegistrations>();
            builder.RegisterModule(new CqrsRegistrationModule(typeof(Program).Assembly));

            AppContainer = builder.Build();
        }

        public void Start()
        {
            var url = _config.WebAppUrl;
            _logger.Information("Starting WebApp on {url}", url);
            _webApp = WebApp.Start(url, iAppBuilder => _owinStartup.Configuration(iAppBuilder));
            _logger.Information("WebApp Running on {url}", url);
        }

        public void Stop()
        {
            _webApp.Dispose();
            _logger.Information("Stopping WebApp on {url}", _config.WebAppUrl);
        }
    }
}
