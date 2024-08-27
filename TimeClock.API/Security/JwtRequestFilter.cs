using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using TimeClock.Api.Helpers;

namespace TimeClock.Api.Security;

/// <summary>
/// Allows expired JWT token on endpoints decorated with <see cref="IAllowExpiredJwtAttribute"/>
/// </summary>
public class JwtRequestFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Endpoint? endpoint = context.HttpContext.GetEndpoint();
        IAllowExpiredJwtAttribute? allowJwtExp = null;
        AuthenticateResult jwt;

        if (endpoint is null)
            throw new Exception("Endpoint not found");

        // Try JWT Bearer authentication first
        jwt = await context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        
        IAllowAnonymous? allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();
        allowJwtExp = endpoint.Metadata.GetMetadata<IAllowExpiredJwtAttribute>();

        if (allowAnonymous != null)
        {
            return await next(context);
            //return AuthenticateResult.NoResult();
        }

        // make sure device id is passed in, matches jwt audience and exists in allowed audiences
#warning missing JWT audience valdidation

        // If jwt failed, we dont need to check anything
        if (jwt.Failure is not null || !jwt.Succeeded)
            return AuthenticateResult.Fail(jwt.Failure ?? new SecurityTokenException("Unknown failure"));

        // Check if expiration doesnt exist
        if (!jwt.Properties.ExpiresUtc.HasValue)
            return AuthenticateResult.Fail(new SecurityTokenExpiredException("Expiration not present"));

        // Check if date is not expired.
        if (jwt.Properties.ExpiresUtc.Value > DateTimeOffset.UtcNow)
            return await next(context);
            //return AuthenticateResult.NoResult();

        // check if this is an exception to expired jwt
        if (allowJwtExp is not null)
            return await next(context);
            //return AuthenticateResult.NoResult();
        else
            return AuthenticateResult.Fail(new SecurityTokenExpiredException($"Expired: {jwt.Properties.ExpiresUtc}"));
    }
}
