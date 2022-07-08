namespace TheOmenDen.EventStore.Aggregates;
#nullable disable
internal static class AggregateFactory<T>
{
    private static readonly Func<T> Constructor = CreateTypeConstructor();

    internal static T CreateAggregate()
    {
        if (Constructor is null)
        {
            throw new MissingDefaultConstructorException(typeof(T));
        }

        return Constructor();
    }

    private static Func<T> CreateTypeConstructor()
    {
        try
        {
            var expression = Expression.New(typeof(T));

            var func = Expression.Lambda<Func<T>>(expression);

            return func.Compile();
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}