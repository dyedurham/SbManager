using System;
using Autofac;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Nancy.Bootstrapper;
using SbManager.BusHelpers;
using SbManager.CQRS.Commands;
using SbManager.CQRS.Extensions;
using SbManager.CQRS.ModelBuilders;
using SbManager.Extensions;
using SbManager.HttpModules.V1;
using SbManager.InternalCommandHandlers;
using SbManager.Models.ViewModelBuilders;
using Serilog;
using Topshelf;

namespace SbManager.Startup
{
    public class AutofacRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register components here
            builder.RegisterType<Service>().As<IService>();
            builder.RegisterType<WindowsService>().As<ServiceControl>();
            builder.RegisterType<OwinStartup>().As<IOwinStartup>();
            builder.RegisterType<NancyBootstrapper>().As<INancyBootstrapper>();

            builder.RegisterType<Config>().As<IConfig>();

            var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.WithThreadId()                      //Add threadId for each log entry
                    .Enrich.FromLogContext()                    //Allow to add context values
                    .Enrich.WithProperty("RuntimeVersion", Environment.Version)
                    .WriteTo.FileSinkDefinedFromConfig()
                    .WriteTo.LiterateConsole();

            Log.Logger = loggerConfiguration.CreateLogger();

            builder.RegisterInstance(Log.Logger).As<ILogger>();

            builder.RegisterType<RequeueAndRemove>().As<IRequeueAndRemove>();
            builder.RegisterType<BusMonitor>().As<IBusMonitor>().SingleInstance();
            builder.RegisterType<Sender>().As<ISender>().SingleInstance();

            builder.Register(c => NamespaceManager.CreateFromConnectionString(c.Resolve<IConfig>().BusConnectionString)).As<NamespaceManager>();
            builder.Register(c => MessagingFactory.CreateFromConnectionString(c.Resolve<IConfig>().BusConnectionString)).As<MessagingFactory>();

            builder.RegisterType<BusManagerModule>().AsSelf().AsImplementedInterfaces();

            builder.RegisterAllImplementationsInAssembly<IModelBuilderBase>(typeof(QueueMessagesBuilder).Assembly);
            builder.RegisterAllImplementationsInAssembly<ICommandHandlerBase>(typeof(RequeueMessageCommandHandler).Assembly);
        }
    }
}

