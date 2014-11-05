//

using System.Reflection;
using Autofac;
using SbManager.CQRS.Commands;
using SbManager.CQRS.Extensions;
using SbManager.CQRS.ModelBuilders;
using Module = Autofac.Module;

namespace SbManager.CQRS
{
    public class CqrsRegistrationModule : Module
    {
        private readonly Assembly[] _assembliesContainingHandlersAndBuilders;

        public CqrsRegistrationModule(params Assembly[] assembliesContainingHandlersAndBuilders)
        {
            _assembliesContainingHandlersAndBuilders = assembliesContainingHandlersAndBuilders;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ModelCreator>().As<IModelCreator>().InstancePerLifetimeScope();
            builder.RegisterType<CommandSender>().As<ICommandSender>().InstancePerLifetimeScope();

            foreach (var assembly in _assembliesContainingHandlersAndBuilders)
            {
                builder.RegisterAllImplementationsInAssembly<ICommandHandlerBase>(assembly);
                builder.RegisterAllImplementationsInAssembly<IModelBuilderBase>(assembly);
            }
        }
    }
}
