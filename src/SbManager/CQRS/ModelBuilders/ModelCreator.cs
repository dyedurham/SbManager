using System;
using Autofac;

namespace SbManager.CQRS.ModelBuilders
{
    public class ModelCreator : IModelCreator
    {
        private readonly ILifetimeScope _lifetimeScope;

        public ModelCreator(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public TModel Build<TModel>() where TModel : class
        {
            var builder = _lifetimeScope.Resolve<IModelBuilder<TModel>>();

            if (builder == null) throw new InvalidOperationException("Could not resolve  builder for Model type " + typeof(TModel).FullName);

            var model = builder.Build();
            return model;
        }

        public TModel Build<TModel, TCriteria>(TCriteria criteria)
            where TCriteria : class
            where TModel : class
        {
            if (criteria == null) throw new ArgumentNullException("criteria");

            var builder = _lifetimeScope.Resolve<IModelBuilderWithCriteria<TModel, TCriteria>>();

            if (builder == null) throw new InvalidOperationException("Could not resolve  builder for Model type " + typeof(TModel).FullName + " and criteria type " + criteria.GetType().FullName);

            return builder.Build(criteria);
        }
    }
}
