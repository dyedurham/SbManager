using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Serilog;

namespace SbManager.Startup
{
    public class NancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope _lifeTimeScope;
        private readonly ILogger _logger;

        public NancyBootstrapper(ILifetimeScope lifeTimeScope, ILogger logger)
        {
            StaticConfiguration.DisableErrorTraces = false;
            _lifeTimeScope = lifeTimeScope;
            _logger = logger.ForContext<NancyBootstrapper>();
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            // Return application container instance
            return _lifeTimeScope;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
            _logger.Information("Nancy starting up");

            pipelines.OnError += (ctx, exception) =>
            {
                ctx.Items.Add("OnErrorException", exception);
                return null;
            };

        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            // Perform registration that should have an application lifetime
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
            //_logger.ForContext("NancyContext", context, true).Debug("Request received");
        }
    }
}

