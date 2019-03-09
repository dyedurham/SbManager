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

                hostConfig.Service<IService>(serviceConfig =>
                {
                    // Let Topshelf use the Autofac Container
                    serviceConfig.ConstructUsingAutofacContainer();
                    serviceConfig.WhenStarted(service => service.Start());
                    serviceConfig.WhenStopped(service => service.Stop());
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

