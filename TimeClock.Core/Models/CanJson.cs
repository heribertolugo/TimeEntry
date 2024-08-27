using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using TimeClock.Core.Security;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models;

public interface ICanJson<T> where T : ICanJson<T>
{
    /// <summary>
    /// Converts this object into a JSON serialized string
    /// </summary>
    /// <returns></returns>
    string AsJson();
    /// <summary>
    /// Converts this object into a Base64 JSON serialized string.
    /// </summary>
    /// <param name="htmlEncode">Whether to encode the wrapped string value for transmission by http (HTML encode).</param>
    /// <param name="cryptographyService">A <see cref="ICryptographyService"/> to use for encrypting the wrapped string value.</param>
    /// <returns></returns>
    public string Wrap(bool htmlEncode = false, ICryptographyService? cryptographyService = null);
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> from a serialized JSON string 
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static abstract T? FromJson(string json);
    /// <summary>
    /// Creates an instance of from a base64 encoded serialized JSON string. 
    /// If <paramref name="isHtmlEncoded"/> is <see langword="true"/>, the provided string will first be HTML Decoded. 
    /// If <paramref name="cryptographyService"/> is provided, the provided string will de decrypted after HTML decoding (if <paramref name="isHtmlEncoded"/> is set).
    /// </summary>
    /// <param name="value">A string providing a value to unwrap into an object instance of <typeparamref name="T"/>, where <typeparamref name="T"/> implements <see cref="ICanJson{T}"/>.</param>
    /// <param name="isHtmlEncoded">Whether the string should be HTML decoded.</param>
    /// <param name="cryptographyService">A <see cref="ICryptographyService"/> instance to use for decypting if provided value is encrypted.</param>
    /// <returns></returns>
    public static abstract T? Unwrap(string value, bool isHtmlEncoded = false, ICryptographyService? cryptographyService = null);
}

/// <summary>
/// Base class for <see cref="ICanJson{T}"/>, providing default implementations.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CanJson<T> : ICanJson<T> where T : ICanJson<T>
{
    /// <summary>
    /// <inheritdoc cref="ICanJson{T}.AsJson"/>
    /// </summary>
    /// <returns></returns>
    public virtual string AsJson() => JsonConvert.SerializeObject(this, Formatting.None);
    /// <summary>
    /// <inheritdoc cref="ICanJson{T}.Wrap(bool, ICryptographyService?)"/>
    /// </summary>
    /// <param name="htmlEncode"><inheritdoc cref="ICanJson{T}.Wrap(bool, ICryptographyService?)"/></param>
    /// <param name="cryptographyService"><inheritdoc cref="ICanJson{T}.Wrap(bool, ICryptographyService?)"/></param>
    /// <returns></returns>
    public virtual string Wrap(bool htmlEncode = false, ICryptographyService? cryptographyService = null)
    {
        string wrappedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.AsJson()));
        if (cryptographyService is not null)
            wrappedValue = cryptographyService.Encrypt(wrappedValue);
        if (htmlEncode)
            wrappedValue = Uri.EscapeDataString(wrappedValue);
        return wrappedValue;
    }
    /// <summary>
    /// <inheritdoc cref="ICanJson{T}.FromJson(string)"/>
    /// </summary>
    /// <param name="json"><inheritdoc cref="ICanJson{T}.FromJson(string)"/></param>
    /// <returns></returns>
    public static T? FromJson(string json) => JsonConvert.DeserializeObject<T>(json);
    /// <summary>
    /// <inheritdoc cref="ICanJson{T}.Unwrap(string, bool, ICryptographyService?)"/>
    /// </summary>
    /// <param name="value"><inheritdoc cref="ICanJson{T}.Unwrap(string, bool, ICryptographyService?)"/></param>
    /// <param name="isHtmlEncoded"><inheritdoc cref="ICanJson{T}.Unwrap(string, bool, ICryptographyService?)"/></param>
    /// <param name="cryptographyService"><inheritdoc cref="ICanJson{T}.Unwrap(string, bool, ICryptographyService?)"/></param>
    /// <returns></returns>
    public static T? Unwrap(string value, bool isHtmlEncoded = false, ICryptographyService? cryptographyService = null)
    {
        if (isHtmlEncoded)
            value = Uri.UnescapeDataString(value);
        if (cryptographyService is not null)
            value = cryptographyService.Decrypt(value);
        return CanJson<T>.FromJson(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
    }
}
