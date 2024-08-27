using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using TimeClock.Core.Security;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

/// <summary>
/// Package which create an object for a response which complies with RFC6750. See <see href="https://datatracker.ietf.org/doc/html/rfc6750#section-4"/>
/// </summary>
/// <param name="Token">A authentication token</param>
/// <param name="Expiration">The expiration of the Authentication token</param>
/// <param name="RefreshToken">A token used for refreshing</param>
/// <param name="Type">The type of Authentication token, defaults to Bearer</param>
internal sealed record class TokenPackage(
    [property: JsonProperty("access_token")][property: JsonPropertyName("access_token")] string Token,
    [property: JsonProperty("expires_in")][property: JsonPropertyName("expires_in")] DateTime Expiration,
    [property: JsonProperty("refresh_token")][property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonProperty("refresh_expires")][property: JsonPropertyName("refresh_expires")] DateTime RefreshExpiration,
    [property: JsonProperty("token_type")][property: JsonPropertyName("token_type")] string Type = "Bearer") : ICanJson<TokenPackage>
{
    public static TokenPackage? FromJson(string json) => CanJson<TokenPackage>.FromJson(json);

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.None);
    }
    public string Wrap(bool htmlEncode = false, ICryptographyService? cryptographyService = null)
    {
        string wrappedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.AsJson()));
        if (cryptographyService is not null)
            wrappedValue = cryptographyService.Encrypt(wrappedValue);
        if (htmlEncode)
            wrappedValue = Uri.EscapeDataString(wrappedValue);
        return wrappedValue;
    }
    public static TokenPackage? Unwrap(string value, bool isHtmlEncoded = false, ICryptographyService? cryptographyService = null) => CanJson<TokenPackage>.Unwrap(value, isHtmlEncoded, cryptographyService);
}
