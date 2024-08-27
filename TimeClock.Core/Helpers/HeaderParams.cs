using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core;

internal static class HeaderParams
{
    /// <summary>
    /// "access_token" Name descriptor used for JWT transmission as defined in RFC6750, see <see href="https://datatracker.ietf.org/doc/html/rfc6750#section-2.2"/>
    /// </summary>
    public static readonly string TokenParameter = "access_token";
    /// <summary>
    /// "refresh_token" Name descriptor used for JWT transmission as defined in RFC6749, see <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-6"/>
    /// </summary>
    public static readonly string RefreshTokenParameter = "refresh_token";
    /// <summary>
    /// "Bearer " Prefix used for JWT transmission as deifined in RFC6750, see <see href="https://datatracker.ietf.org/doc/html/rfc6750#section-2.1"/>
    /// </summary>
    public static readonly string TokenTypePrefix = "Bearer";
    /// <summary>
    /// TC_Token
    /// </summary>
    public static readonly string CustomTokenParameter = "TC_token";
    /// <summary>
    /// TC_pkey
    /// </summary>
    public static readonly string CustomCryptoKeyParameter = "TC_pkey";
}
