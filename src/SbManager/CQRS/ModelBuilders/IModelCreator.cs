using System.Threading.Tasks;

namespace SbManager.CQRS.ModelBuilders
{
    public interface IModelCreator
    {
        Task<TModel> Build<TModel>()
            where TModel : class;
        Task<TModel> Build<TModel, TCriteria>(TCriteria criteria)
            where TCriteria : class
            where TModel : class;
    }

    public interface IModelBuilderBase { }

    public interface IModelBuilder<TModel> : IModelBuilderBase
        where TModel : class
    {
        Task<TModel> Build();
    }

    public interface IModelBuilderWithCriteria<TModel, in TCriteria> : IModelBuilderBase
        where TCriteria : class
        where TModel : class
    {
        Task<TModel> Build(TCriteria criteria);
    }
}
