namespace TimeClock.JdeSync.Helpers;
internal static class CollectionExtensions
{
    public static BiDirectionalEnumerable<T> ToBiDirectionalEnumerable<T>(this IEnumerable<T> values)
    {
        return new BiDirectionalEnumerable<T>(values);
    }
    /// <summary>
    /// Performs a cast of the <typeparamref name="TIn"/> provided into the <typeparamref name="TOut"/> type, 
    /// and then returns the <see cref="IEnumerable{T}"/> as a <see cref="BiDirectionalEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static BiDirectionalEnumerable<TOut> ToBiDirectionalEnumerable<TIn,TOut>(this IEnumerable<TIn> values)
    {
        return new BiDirectionalEnumerable<TOut>(values.Cast<TOut>());
    }
}
