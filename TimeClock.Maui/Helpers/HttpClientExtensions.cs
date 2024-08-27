using System.Collections.Specialized;
using System.Web;

namespace TimeClock.Maui.Helpers;

internal static class HttpClientExtensions
{
    public static string AddParameters(this HttpClient client, params (string Name, string Value)[] @params)
    {
        if (@params.Length == 0)
            return string.Empty;

        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

        foreach ((string Name, string Value) param in @params)
        {
            query[param.Name] = param.Value.Trim();
        }

        return query.ToString()!;
    }
}
