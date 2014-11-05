namespace SbManager.CQRS.ModelBuilders
{
    public interface IModelCreator
    {
        TModel Build<TModel>()
            where TModel : class;
        TModel Build<TModel, TCriteria>(TCriteria criteria)
            where TCriteria : class
            where TModel : class;
    }

    public interface IModelBuilderBase { }

    public interface IModelBuilder<out TModel> : IModelBuilderBase
        where TModel : class
    {
        TModel Build();
    }

    public interface IModelBuilderWithCriteria<out TModel, in TCriteria> : IModelBuilderBase
        where TCriteria : class
        where TModel : class
    {
        TModel Build(TCriteria criteria);
    }
}
