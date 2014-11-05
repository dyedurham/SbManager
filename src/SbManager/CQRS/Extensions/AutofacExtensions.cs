using System.Linq;
using System.Reflection;
using Autofac;

namespace SbManager.CQRS.Extensions
{
    public static class AutofacExtensions
    {
        public static void RegisterAllImplementationsInAssembly<TInterface>(this ContainerBuilder builder, Assembly assembly, bool propertiesAutowired = false)
        {
            assembly
                .GetTypes()
                .Where(t => typeof(TInterface).IsAssignableFrom(t) && !t.IsInterface)
                .ToList().ForEach(t => t.GetInterfaces().ToList().ForEach(i =>
                {
                    var reg = builder.RegisterType(t).As(i).InstancePerLifetimeScope();
                    if (propertiesAutowired) reg.PropertiesAutowired();
                }));
        }
    }
}
