namespace SbManager.CQRS
{
    public class ValueCriteria<TValueType>
    {
        public ValueCriteria(TValueType val)
        {
            Value = val;
        }

        public TValueType Value { get; protected set; }

        public static ValueCriteria<TValueType> From(TValueType val)
        {
            return new ValueCriteria<TValueType>(val);
        }
    }
}
