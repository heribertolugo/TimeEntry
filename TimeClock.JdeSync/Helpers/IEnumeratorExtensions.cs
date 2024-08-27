namespace TimeClock.JdeSync.Helpers;
internal static class IEnumeratorExtensions
{
    /// <summary>
    /// Returns current or default without allowing exception to be thrown. 
    /// Often happens when trying to get current on empty collection. 
    /// See: <see href="https://github.com/dotnet/runtime/issues/94256">GitHub Issue 94256</see>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    public static T? GetCurrent<T>(this IEnumerator<T> enumerator)
    {
        // we need to do this because Microsoft are really hardheaded 1d10t5 sometimes
        // new behavior in .net core 8 
        // trying to access current after enumeration has finished causes exception.
        try
        {
            return enumerator.Current;
        } 
        catch(InvalidOperationException)
        {
            return default;
        }
    }
}
