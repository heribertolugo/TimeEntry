using Microsoft.AspNetCore.Authentication.JwtBearer;
using CommonCore = TimeClock.Core.HeaderParams;

namespace TimeClock.Api.Helpers
{
    public static class ContextExtensions
    {
        public static void Set(this IHeaderDictionary headers, string key, string value)
        {
            if (headers.ContainsKey(key))
                headers[key] = value;
            else headers.Append(key, value);
        }

        public static string? GetBearer(this IHeaderDictionary headers)
        {
            string? authorizationHeader = headers.Authorization.FirstOrDefault();

            if (authorizationHeader is null) return null;

            return authorizationHeader.StartsWith(JwtBearerDefaults.AuthenticationScheme) 
                ? authorizationHeader[JwtBearerDefaults.AuthenticationScheme.Length..].Trim() 
                : authorizationHeader;
        }

        public static string? GetRefreshToken(this IHeaderDictionary headers)
        {
            string? authorizationHeader = headers.Authorization.FirstOrDefault();

            if (authorizationHeader is null)
            {
                return headers[CommonCore.RefreshTokenParameter];
            }

            return authorizationHeader.StartsWith(CommonCore.RefreshTokenParameter)
                ? authorizationHeader[CommonCore.RefreshTokenParameter.Length..].Trim()
                : authorizationHeader;
        }
    }
}
