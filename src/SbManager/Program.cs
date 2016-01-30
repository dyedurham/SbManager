using System.IO;
using SbManager.Startup;
using Serilog;
using Topshelf;
using Topshelf.Autofac;

namespace SbManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Service.Setup();
            HostFactory.Run(hostConfig =>
            {
                hostConfig.UseSerilog(Log.Logger);

                // Pass the Autofac container to Topshelf
                hostConfig.UseAutofacContainer(Service.AppContainer);

                hostConfig.Service<ServiceControl>(serviceConfig =>
                {
                    // Let Topshelf use the Autofac Container
                    serviceConfig.ConstructUsingAutofacContainer();
                    serviceConfig.WhenStarted((service, control) => service.Start(control));
                    serviceConfig.WhenStopped((service, control) => service.Stop(control));
                });

                hostConfig.SetDescription(Constants.AppDescription);
                hostConfig.SetDisplayName(Constants.AppName);

                var serviceFilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                var serviceFileName = Path.GetFileName(serviceFilePath);

                hostConfig.SetServiceName(serviceFileName);
            });
        }
    }
}

